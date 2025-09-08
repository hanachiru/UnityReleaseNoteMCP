using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnityReleaseNoteMCP.Services;

namespace UnityReleaseNoteMCP.Endpoints;

public static class GetUnityReleasesEndpoint
{
    public static void MapGetUnityReleasesEndpoint(this WebApplication app)
    {
        app.MapGet("/unity/editor/release/v1/releases", async (
            [FromServices] IUnityReleaseService service,
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0,
            [FromQuery] string? order = "RELEASE_DATE_DESC",
            [FromQuery] string[]? stream = null,
            [FromQuery] string[]? platform = null,
            [FromQuery] string[]? architecture = null,
            [FromQuery] string? version = null) =>
        {
            var result = await service.GetUnityReleasesAsync(limit, offset, order, stream, platform, architecture, version);
            return Results.Ok(result);
        })
        .WithName("GetUnityReleases")
        .WithSummary("Get Unity Releases")
        .WithDescription("Get a list of Unity Editor releases with optional filtering and pagination.")
        .WithOpenApi();
    }
}
