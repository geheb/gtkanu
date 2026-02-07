namespace GtKanu.Infrastructure.Security;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public sealed class BlockerMiddleware
{
    private readonly RequestDelegate _next;

    public BlockerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var address = context.Connection.RemoteIpAddress;
        if (address is null || IPAddress.IsLoopback(address))
        {
            await _next(context);
            return;
        }

        var blacklist = context.RequestServices.GetRequiredService<BlacklistCache>();

        string? userAgent = context.Request.Headers.UserAgent;
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            var botItem = blacklist.Update(address, null, true);
            await CreateBannedResponse(context, botItem.Count > 3);
            return;
        }

        var item = blacklist.Get(address, userAgent);
        if (item?.IsSuspicious == true)
        {
            await CreateBannedResponse(context, item.IsCulprit);
            return;
        }

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            var checker = context.RequestServices.GetRequiredService<IpReputationChecker>();
            var isListed = await checker.IsListed(address);
            blacklist.Update(address, userAgent, isListed);
        }
    }

    private static async Task CreateBannedResponse(HttpContext context, bool shouldAbortConnection)
    {
        context.Response.StatusCode = StatusCodes.Status418ImATeapot;
        context.Response.Headers["Connection"] = "close";
        await context.Response.WriteAsync("You are banned on this site!", context.RequestAborted);

        if (shouldAbortConnection)
        {
            var connection = context.Features.Get<IConnectionLifetimeFeature>();
            if (connection is not null)
            {
                connection.Abort();
            }
            else
            {
                context.Abort();
            }
        }
    }
}
