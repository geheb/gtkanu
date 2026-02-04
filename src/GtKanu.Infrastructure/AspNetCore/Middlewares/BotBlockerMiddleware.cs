namespace GtKanu.Infrastructure.AspNetCore.Middlewares;

using GtKanu.Application.Converter;
using GtKanu.Infrastructure.Email;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public sealed class BotBlockerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly GermanDateTimeConverter _dateTimeConverter = new();

    public BotBlockerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var now = _dateTimeConverter.ToLocal(DateTimeOffset.UtcNow);
        var isWorkingTime = now.Hour >= 6 && now.Hour <= 21;
        if (isWorkingTime)
        {
            return;
        }

        var address = context.Connection.RemoteIpAddress;
        if (address is null || IPAddress.IsLoopback(address))
        {
            await _next(context);
            return;
        }

        var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
        var key = "bot-" + address;

        if (await HandleBanned(key, context, cache))
        {
            return;
        }

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            var checker = context.RequestServices.GetRequiredService<IpReputationChecker>();
            if (await HandleListed(key, context, address, cache, checker))
            {
                await HandleBanned(key, context, cache);
            }
        }
    }

    private static async Task<bool> HandleListed(string key, HttpContext context, IPAddress address, IMemoryCache cache, IpReputationChecker checker)
    {
        if (await checker.IsListed(address))
        {
            cache.Set(key, 7, DateTimeOffset.UtcNow.AddHours(1));
            return true;
        }
        else
        {
            var nextCounter = cache.TryGetValue(key, out int counter)
                ? counter + 1
                : 1;

            cache.Set(key, nextCounter, DateTimeOffset.UtcNow.AddHours(1));

            return false;
        }
    }

    private static async Task<bool> HandleBanned(string key, HttpContext context, IMemoryCache cache)
    {
        if (!cache.TryGetValue(key, out int counter) || counter < 7)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status418ImATeapot;
        context.Response.Headers["Connection"] = "close";

        if (counter == 7)
        {
            cache.Set(key, counter + 1, DateTimeOffset.UtcNow.AddHours(1));
            await context.Response.WriteAsync("You are banned on this site!", context.RequestAborted);
            return true;
        }

        var connection = context.Features.Get<IConnectionLifetimeFeature>();
        if (connection is not null)
        {
            connection.Abort();
        }
        else
        {
            context.Abort();
        }

        return true;
    }
}
