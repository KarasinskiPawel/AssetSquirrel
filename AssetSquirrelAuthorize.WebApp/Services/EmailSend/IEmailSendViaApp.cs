namespace AssetSquirrelAuthorize.WebApp.Services.EmailSend
{
    public interface IEmailSendViaApp
    {
        bool Send(EmailMessage message, bool isHtml = false);
    }
}
