using AssetSquirrelAuthorize.WebApp.Services.EmailSend;
using AssetsSquirrel.CoreBusiness;
using Microsoft.AspNetCore.Identity;

namespace AssetSquirrelAuthorize.WebApp.Components.Account
{
    internal sealed class SmtpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailSendViaApp _emailSendViaApp;

        public SmtpEmailSender(IEmailSendViaApp emailSendViaApp)
        {
            _emailSendViaApp = emailSendViaApp;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Potwierdź swoje konto", $"Potwierdź swoje konto, <a href='{confirmationLink}'>klikając tutaj</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Zresetuj hasło", $"Zresetuj hasło, <a href='{resetLink}'>klikając tutaj</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Zresetuj hasło", $"Twój kod do zresetowania hasła: {resetCode}");

        // Delegates the actual SMTP transmission to EmailSendViaApp -- ported
        // from a separate application that already sends successfully through
        // the same mailbox/relay (see AssetSquirrelAuthorize.WebApp/Services/
        // EmailSend). EmailSendViaApp logs failures itself, so this only needs
        // to translate a false return into the exception the Account pages'
        // try/catch already expects.
        private Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new EmailMessage
            {
                Subject = subject,
                Body = htmlMessage,
                Recipients = new[] { toEmail }
            };

            if (!_emailSendViaApp.Send(message, isHtml: true))
            {
                throw new InvalidOperationException("Failed to send email.");
            }

            return Task.CompletedTask;
        }
    }
}
