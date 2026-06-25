using MediatR;
using ProfileService.Features.Profile;
using ProfileService.Features.Settings.GetSettings;
using ProfileService.Features.Settings.UpdateSettings;

namespace ProfileService.Features.Settings;

public static class SettingsEndpoints
{
    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/settings")
            .RequireAuthorization()
            .WithTags("Settings");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetSettingsQuery(), ct)).ToHttpResult());

        group.MapPut("", async (UpdateSettingsCommand request, ISender sender, CancellationToken ct) =>
            (await sender.Send(request, ct)).ToHttpResult());

        return app;
    }
}
