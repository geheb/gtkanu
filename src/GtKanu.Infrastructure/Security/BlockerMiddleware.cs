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
        int score;
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            score = blacklist.Set(address);
            if (await TryCreateBannedResponse(context, score))
            {
                return;
            }
            await _next(context);
            return;
        }

        score = blacklist.Get(address, userAgent);
        if (await TryCreateBannedResponse(context, score))
        { 
            return;
        }

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            var checker = context.RequestServices.GetRequiredService<IpReputationChecker>();
            if (await checker.GetBlacklisted(address))
            {
                blacklist.Set(address, userAgent);
            }
        }
    }

    private static async Task<bool> TryCreateBannedResponse(HttpContext context, int score)
    {
        if (score < 7)
        {
            return false;
        }

        if (score > 7)
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
        else
        {
            context.Response.StatusCode = StatusCodes.Status418ImATeapot;
            context.Response.Headers["Connection"] = "close";
            await context.Response.WriteAsync("You are banned on this site!", context.RequestAborted);
        }
        return true;
    }
}
