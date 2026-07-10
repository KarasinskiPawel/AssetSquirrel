using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.Extensions.Configuration;

namespace AssetSquirrelAuthorize.WebApp.Services.EmailSend
{
    // Ported from iKomfortCore.ApplicationGlobal.EmailManager.EmailSendViaApp (a
    // separate, already-working application against the same mailbox/relay) --
    // same SmtpClient setup and synchronous Send(), adapted to read Host/Port/
    // From/Username/Password from configuration (see appsettings.json's "Smtp"
    // section + user-secrets/appsettings.Production.json) instead of a hardcoded
    // password, and to log failures via this project's IErrorsRepository instead
    // of iKomfortCore's ErrorHandlingStatic.
    public class EmailSendViaApp : IEmailSendViaApp
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorsRepository _errorsRepository;

        public EmailSendViaApp(IConfiguration configuration, IErrorsRepository errorsRepository)
        {
            _configuration = configuration;
            _errorsRepository = errorsRepository;
        }

        public bool Send(EmailMessage message, bool isHtml = false)
        {
            if (message == null)
                return false;

            var smtp = _configuration.GetSection("Smtp");
            var fromAddress = new MailAddress(smtp["From"] ?? string.Empty, "Nadawca");

            bool output = true;

            var client = new SmtpClient
            {
                Host = smtp["Host"] ?? string.Empty,
                Port = smtp.GetValue<int>("Port"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"])
            };

            try
            {
                var mail = new MailMessage
                {
                    From = fromAddress,
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = isHtml
                };

                if (message.Recipients != null)
                {
                    foreach (var addr in message.Recipients)
                        if (IsValidEmail(addr))
                            mail.To.Add(addr);
                }

                if (message.CC != null)
                {
                    foreach (var cc in message.CC)
                        if (IsValidEmail(cc))
                            mail.CC.Add(cc);
                }

                if (!string.IsNullOrWhiteSpace(message.AttachmentPath))
                {
                    var attachment = new Attachment(
                        message.AttachmentPath,
                        MediaTypeNames.Application.Octet);

                    mail.Attachments.Add(attachment);
                }

                client.Send(mail);
            }
            catch (Exception e)
            {
                _errorsRepository.AddErrorAsync(
                    "AssetSquirrelAuthorize.WebApp.Services.EmailSend",
                    nameof(EmailSendViaApp),
                    nameof(Send),
                    e).GetAwaiter().GetResult();

                output = false;
            }

            return output;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
