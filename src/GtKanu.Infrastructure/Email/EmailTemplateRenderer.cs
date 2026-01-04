using GtKanu.Application.Models;
using System.Reflection;

namespace GtKanu.Infrastructure.Email;

public class EmailTemplateRenderer
{
    private readonly TemplateRenderer _templateRenderer;

    public EmailTemplateRenderer()
    {
        _templateRenderer = new TemplateRenderer(GetType().GetTypeInfo().Assembly);
    }

    public string Render(EmailTemplate emailTemplate, object model)
    {
        var templateFile = GetTemplateFile(emailTemplate);
        return _templateRenderer.Render(templateFile, model);
    }

    private static string GetTemplateFile(EmailTemplate emailTemplate)
    {
        return emailTemplate switch
        {
            EmailTemplate.ConfirmRegistration => "ConfirmRegistration.html",
            EmailTemplate.ConfirmRegistrationExtended => "ConfirmRegistrationExtended.html",
            EmailTemplate.ConfirmPasswordForgotten => "ConfirmPasswordForgotten.html",
            EmailTemplate.ConfirmChangeEmail => "ConfirmChangeEmail.html",
            EmailTemplate.Mailing => "Mailing.html",
            _ => throw new NotImplementedException($"unknown {nameof(EmailTemplate)} {emailTemplate}")
        };
    }
}
