using System.Collections.Generic;

namespace AssetSquirrelAuthorize.WebApp.Services.EmailSend
{
    public class EmailMessage
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public IEnumerable<string>? Recipients { get; set; }
        public IEnumerable<string>? CC { get; set; }
        public string? AttachmentPath { get; set; }
    }
}
