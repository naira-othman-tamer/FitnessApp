using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure;
using Repository.Layer.Interfaces;

namespace ProfileService.Features.Profile.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber) : IRequest<OperationResult<ProfileResponse>>;

public sealed record ProfileResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? ProfilePictureUrl,
    bool IsPremiumCached,
    DateTime MemberSince);

public sealed partial class UpdateProfileHandler(
    IClaimsManager claims,
    IUnitOfWork<ProfileDbContext> unitOfWork) : IRequestHandler<UpdateProfileCommand, OperationResult<ProfileResponse>>
{
    public async Task<OperationResult<ProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var error = Validate(request);
        if (error is not null)
            return OperationResultFactory.BadRequest<ProfileResponse>(error, error);

        var userId = claims.UserId;
        var profile = await unitOfWork.Repository<UserProfile, Guid>()
            .Query(asNoTracking: false)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId = userId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PhoneNumber = request.PhoneNumber.Trim(),
                MemberSince = DateTime.UtcNow,
                Preferences = new UserPreferences(),
                NotificationSettings = new NotificationSettings(),
                PrivacySettings = new PrivacySettings()
            };
            await unitOfWork.Repository<UserProfile, Guid>().Create(profile);
        }
        else
        {
            profile.FirstName = request.FirstName.Trim();
            profile.LastName = request.LastName.Trim();
            profile.Email = request.Email.Trim().ToLowerInvariant();
            profile.PhoneNumber = request.PhoneNumber.Trim();
        }

        await unitOfWork.CompleteAsync();

        return OperationResultFactory.Success(new ProfileResponse(
            profile.UserId,
            profile.FirstName,
            profile.LastName,
            profile.Email,
            profile.PhoneNumber,
            profile.ProfilePictureUrl,
            profile.IsPremiumCached,
            profile.MemberSince));
    }

    private static string? Validate(UpdateProfileCommand request)
    {
        if (request.FirstName.Trim().Length is < 2 or > 50 || request.LastName.Trim().Length is < 2 or > 50)
            return "First and last names must each contain between 2 and 50 characters.";
        if (!new EmailAddressAttribute().IsValid(request.Email))
            return "A valid email is required.";
        if (!EgyptianPhoneRegex().IsMatch(request.PhoneNumber))
            return "Phone number must be a valid Egyptian mobile number in international format.";
        return null;
    }

    [GeneratedRegex(@"^\+20(?:10|11|12|15)\d{8}$")]
    private static partial Regex EgyptianPhoneRegex();
}
