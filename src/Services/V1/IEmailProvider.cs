using EmailSenderAPI.Models;
using EmailSenderAPI.Models.V1;

namespace EmailSenderAPI.Services.V1
{
    public interface IEmailProvider
    {
        Task<bool> SendEmailAsync(MailData mailData, CancellationToken ct);
        Task<bool> SendWithAttachmentsAsync(MailData mailData, CancellationToken ct);    
        Task<bool> SendHtmlEmailAsync(MailData mailData, CancellationToken ct);
        Task<bool> SendHtmlWithAttachmentsAsync(MailData mailData, CancellationToken ct);

    }
}
