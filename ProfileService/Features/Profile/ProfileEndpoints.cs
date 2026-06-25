using MediatR;
using ProfileService.Features.Profile.ChangePassword;
using ProfileService.Features.Profile.GetProfile;
using ProfileService.Features.Profile.UpdateProfile;
using ProfileService.Features.Profile.UploadProfilePicture;

namespace ProfileService.Features.Profile;

public static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/profile")
            .RequireAuthorization()
            .WithTags("Profile");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetProfileQuery(), ct)).ToHttpResult());

        group.MapPut("", async (UpdateProfileCommand request, ISender sender, CancellationToken ct) =>
            (await sender.Send(request, ct)).ToHttpResult());

        group.MapPost("/picture", async (HttpContext context, ISender sender, CancellationToken ct) =>
            (await sender.Send(new UploadProfilePictureCommand(context), ct)).ToHttpResult());

        group.MapPut("/change-password", async (ChangePasswordCommand request, ISender sender, CancellationToken ct) =>
            (await sender.Send(request, ct)).ToHttpResult());

        return app;
    }
}
