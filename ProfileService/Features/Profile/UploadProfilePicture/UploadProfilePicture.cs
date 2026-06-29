using MediatR;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure;
using Repository.Layer.Interfaces;

namespace ProfileService.Features.Profile.UploadProfilePicture;

public sealed record UploadProfilePictureCommand(HttpContext Context) : IRequest<OperationResult<ProfilePictureResponse>>;

public sealed record ProfilePictureResponse(string ProfilePictureUrl);

public sealed class UploadProfilePictureHandler(
    IWebHostEnvironment environment,
    IClaimsManager claims,
    IUnitOfWork<ProfileDbContext> unitOfWork) : IRequestHandler<UploadProfilePictureCommand, OperationResult<ProfilePictureResponse>>
{
    private static readonly HashSet<string> AllowedContentTypes = ["image/jpeg", "image/png"];
    private const long MaxSizeBytes = 5 * 1024 * 1024;

    public async Task<OperationResult<ProfilePictureResponse>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        if (!request.Context.Request.HasFormContentType)
            return OperationResultFactory.BadRequest<ProfilePictureResponse>("Multipart form data is required.", "Multipart form data is required.");

        var form = await request.Context.Request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("profilePicture");
        if (file is null || file.Length == 0)
            return OperationResultFactory.BadRequest<ProfilePictureResponse>("profilePicture file is required.", "profilePicture file is required.");
        if (file.Length > MaxSizeBytes)
            return OperationResultFactory.BadRequest<ProfilePictureResponse>("Profile picture cannot exceed 5MB.", "Profile picture cannot exceed 5MB.");
        if (!AllowedContentTypes.Contains(file.ContentType))
            return OperationResultFactory.BadRequest<ProfilePictureResponse>("Only JPG and PNG files are allowed.", "Only JPG and PNG files are allowed.");

        var profile = await unitOfWork.Repository<UserProfile, Guid>()
            .Query(asNoTracking: false)
            .SingleOrDefaultAsync(x => x.UserId == claims.UserId, cancellationToken);
        if (profile is null)
            return OperationResultFactory.NotFound<ProfilePictureResponse>("Profile was not found.", "لم يتم العثور على الملف الشخصي");

        var extension = file.ContentType == "image/png" ? ".png" : ".jpg";
        var relativeDirectory = Path.Combine("uploads", "profile-pictures", claims.UserId.ToString("N"));
        var absoluteDirectory = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(absoluteDirectory, fileName);
        await using (var stream = File.Create(absolutePath))
            await file.CopyToAsync(stream, cancellationToken);

        var url = "/" + Path.Combine(relativeDirectory, fileName).Replace('\\', '/');
        profile.ProfilePictureUrl = url;
        await unitOfWork.CompleteAsync();

        return OperationResultFactory.Success(new ProfilePictureResponse(url));
    }
}
