using System.ComponentModel.DataAnnotations;

namespace EmailSenderAPI.Models
{
    public class MailDataDto
    {
        [Required]
        public List<string>? To { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Body { get; set; }     
    }
}
