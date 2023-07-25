using System.ComponentModel.DataAnnotations;

namespace EmailSenderAPI.Models.V1
{
    public class MailDataWithAttachmentsDto
    {
        [Required]
        public List<string>? To { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Body { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
