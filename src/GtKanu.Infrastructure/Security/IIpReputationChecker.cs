namespace GtKanu.Infrastructure.Security;

using System.Net;

internal interface IIpReputationChecker
{
    Task<bool> IsListed(IPAddress address);
    Task<bool> IsListedMx(string domain, CancellationToken cancellationToken);
}
