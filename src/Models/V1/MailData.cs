using EmailSenderAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.ComponentModel.DataAnnotations;

namespace EmailSenderAPI.Models
{
   
    public class MailData 
    {

        // Receiver
        [Required]
        public List<string>? To { get; set; }
        //public List<string> Bcc { get; set; }
        //public List<string> Cc { get; set; }
        
        /*
        //Sender
        public string? From { get; set; }
        public string? DisplayName { get; set; }
        public string? ReplyTo { get; set; }
        public string? ReplyToName { get; set; }
        */
        
        //Content
        public string? Subject { get; set; }
        public string? Body { get; set; }
       
        public List<IFormFile>? Attachments { get; set; }
        
        public MailData Create(List<string>? to,  string? subject = null, string body =null, List<IFormFile> attachments = null)
        {
            MailData mailData = new MailData();
            new MailData() {
                To = to ?? new List<string>(),
                Subject = subject,
                Body = body,
                Attachments = attachments ?? new List<IFormFile>()
            };
            return mailData;
        }
              
    }
}
