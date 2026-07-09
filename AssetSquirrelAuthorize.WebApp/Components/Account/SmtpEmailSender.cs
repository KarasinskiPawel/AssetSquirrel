using System.Net;
using System.Net.Mail;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.CoreBusiness;
using Microsoft.AspNetCore.Identity;

namespace AssetSquirrelAuthorize.WebApp.Components.Account
{
    internal sealed class SmtpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorsRepository _errorsRepository;

        public SmtpEmailSender(
            IConfiguration configuration,
            IErrorsRepository errorsRepository
            )
        {
            _configuration = configuration;
            _errorsRepository = errorsRepository;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Potwierdź swoje konto", $"Potwierdź swoje konto, <a href='{confirmationLink}'>klikając tutaj</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Zresetuj hasło", $"Zresetuj hasło, <a href='{resetLink}'>klikając tutaj</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Zresetuj hasło", $"Twój kod do zresetowania hasła: {resetCode}");

        private async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtp = _configuration.GetSection("Smtp");

            using var client = new SmtpClient
            {
                Host = smtp["Host"] ?? string.Empty,
                Port = smtp.GetValue<int>("Port"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"])
            };
            using var message = new MailMessage(smtp["From"]!, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                await _errorsRepository.AddErrorAsync("SmtpEmailSender", nameof(SmtpEmailSender), nameof(SendEmailAsync), ex);
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
