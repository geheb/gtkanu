namespace GtKanu.Infrastructure.Email;

using GtKanu.Application.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

internal sealed class SmtpDispatcher
{
    private readonly SmtpConnectionOptions _connectionOptions;
    private readonly string? _mailingFooterImagePath;

    public SmtpDispatcher(
        IWebHostEnvironment environment,
        IOptions<AppSettings> appOptions,
        IOptions<SmtpConnectionOptions> smtpOptions)
    {
        _connectionOptions = smtpOptions.Value;
        if (!string.IsNullOrWhiteSpace(appOptions.Value.MailingFooterImageName))
        {
            var path = Path.Combine(
                environment.WebRootPath,
                "html",
                appOptions.Value.MailingFooterImageName);

            if (File.Exists(path))
            {
                _mailingFooterImagePath = path;
            }
        }
    }

    public async Task Send(
        EmailItem[] items,
        CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_connectionOptions.Server, _connectionOptions.Port);
        if (!string.IsNullOrEmpty(_connectionOptions.LoginName))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true; // only Explicit SSL supported
            client.Credentials = new NetworkCredential(_connectionOptions.LoginName, _connectionOptions.LoginPassword);
        }

        foreach (var item in items)
        {
            var from = new MailAddress(_connectionOptions.SenderEmail, _connectionOptions.SenderName);
            var to = new MailAddress(item.Recipient);
            using var message = new MailMessage(from, to);
            if (!string.IsNullOrEmpty(item.ReplyAddress))
            {
                message.ReplyToList.Add(new MailAddress(item.ReplyAddress));
            }
            message.Subject = item.Subject;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            AddBody(message, item.HtmlBody);

            await client.SendMailAsync(message, cancellationToken);
        }        
    }

    public void AddBody(MailMessage message, string htmlBody)
    {
        var endBodyIndex = htmlBody.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(_mailingFooterImagePath) || endBodyIndex < 0)
        {
            message.Body = htmlBody;
            return;
        }

        const string imageTemplate = "<hr/><div style=\"display:flex;justify-content:center;\"><img src=\"cid:footer\" alt=\"\" /></div>";
        htmlBody = htmlBody.Insert(endBodyIndex, imageTemplate);

        var mediaType =
            _mailingFooterImagePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            ? MediaTypeNames.Image.Png
            : MediaTypeNames.Image.Jpeg;

        var image = new LinkedResource(_mailingFooterImagePath, mediaType);
        image.ContentId = "footer";

        var view = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
        view.LinkedResources.Add(image);
        message.AlternateViews.Add(view);
    }
}
