using System.Net;
using System.Net.Mail;
using AssetsSquirrel.CoreBusiness;
using Microsoft.AspNetCore.Identity;

namespace AssetSquirrelAuthorize.WebApp.Components.Account
{
    internal sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Potwierdź swoje konto", $"Potwierdź swoje konto, <a href='{confirmationLink}'>klikając tutaj</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Zresetuj hasło", $"Zresetuj hasło, <a href='{resetLink}'>klikając tutaj</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Zresetuj hasło", $"Twój kod do zresetowania hasła: {resetCode}");

        private Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtp = configuration.GetSection("Smtp");

            using var client = new SmtpClient(smtp["Host"], smtp.GetValue<int>("Port"))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"])
            };

            using var message = new MailMessage(smtp["From"]!, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            return client.SendMailAsync(message);
        }
    }
}
