using MediatR;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure;
using Repository.Layer.Interfaces;

namespace ProfileService.Features.Profile.GetProfile;

public sealed record GetProfileQuery : IRequest<OperationResult<ProfileResponse>>;

public sealed record ProfileResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? ProfilePictureUrl,
    bool IsPremiumCached,
    DateTime MemberSince);

public sealed class GetProfileHandler(
    IClaimsManager claims,
    IUnitOfWork<ProfileDbContext> unitOfWork) : IRequestHandler<GetProfileQuery, OperationResult<ProfileResponse>>
{
    public async Task<OperationResult<ProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await unitOfWork.Repository<UserProfile, Guid>()
            .Query()
            .SingleOrDefaultAsync(x => x.UserId == claims.UserId, cancellationToken);

        return profile is null
            ? OperationResultFactory.NotFound<ProfileResponse>("Profile was not found.", "لم يتم العثور على الملف الشخصي")
            : OperationResultFactory.Success(new ProfileResponse(
                profile.UserId,
                profile.FirstName,
                profile.LastName,
                profile.Email,
                profile.PhoneNumber,
                profile.ProfilePictureUrl,
                profile.IsPremiumCached,
                profile.MemberSince));
    }
}
