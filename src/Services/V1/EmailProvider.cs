using EmailSenderAPI.Models;
using EmailSenderAPI.Models.V1;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Security.Authentication;

namespace EmailSenderAPI.Services.V1
{
    public class EmailProvider : IEmailProvider
    {
        public readonly MailSettings _settings;
        private readonly ILogger<EmailProvider> _logger;

        public EmailProvider(IOptions<MailSettings> settings, ILogger<EmailProvider> logger)
        {
            _logger = logger;
            _settings = settings.Value;
        }


        private MimeMessage CreateEmailMessage(MailData mailData)
        {
            var mailMessage = new MimeMessage();

            //Sender
            mailMessage.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
            mailMessage.Sender = new MailboxAddress(_settings.DisplayName, _settings.From);

            //Receiver
            foreach (string mailAddress in mailData.To)
                mailMessage.To.Add(MailboxAddress.Parse(mailAddress));
            //Add Content to Mime Message

            var body = new BodyBuilder();
            
            //Check if we got any attachments and add the to the builder for our message
            
            if (mailData.Attachments != null)
            {
                byte[] attachmentFileByteArray;

                foreach(IFormFile attacment in mailData.Attachments)
                {
                    //Check if length of the file in bytes is larger than 0
                    if(attacment.Length > 0)
                    {
                        //Create a new memory stream and attach attacments to mail body
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            attacment.CopyTo(memoryStream);
                            attachmentFileByteArray = memoryStream.ToArray();
                        }
                        //Add the attacments from the byte array
                        body.Attachments.Add(attacment.FileName, attachmentFileByteArray, ContentType.Parse(attacment.ContentType));


                    }
                }
            }
            mailMessage.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mailMessage.Body = body.ToMessageBody();
            return mailMessage;
        }

        private MimeMessage HtmlCreateEmailMessage(MailData mailData)
        {
            var mailMessage = new MimeMessage();

            //Sender
            mailMessage.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
            mailMessage.Sender = new MailboxAddress(_settings.DisplayName, _settings.From);

            //Receiver
            foreach (string mailAddress in mailData.To)
                mailMessage.To.Add(MailboxAddress.Parse(mailAddress));
            //Add Content to Mime Message

            

            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\EmailTemplate\\Hello.html";
            string emailTemplateText = File.ReadAllText(filePath);

            emailTemplateText = string.Format(emailTemplateText, mailData.Subject, mailData.Body, DateTime.Today.Date.ToShortDateString());

            var body = new BodyBuilder();
            //Check if we got any attachments and add the to the builder for our message

            if (mailData.Attachments != null)
            {
                byte[] attachmentFileByteArray;

                foreach (IFormFile attacment in mailData.Attachments)
                {
                    //Check if length of the file in bytes is larger than 0
                    if (attacment.Length > 0)
                    {
                        //Create a new memory stream and attach attacments to mail body
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            attacment.CopyTo(memoryStream);
                            attachmentFileByteArray = memoryStream.ToArray();
                        }
                        //Add the attacments from the byte array
                        body.Attachments.Add(attacment.FileName, attachmentFileByteArray, ContentType.Parse(attacment.ContentType));


                    }
                }
            }
            mailMessage.Subject = mailData.Subject;
            body.HtmlBody = emailTemplateText;
            body.TextBody= mailData.Body;
            mailMessage.Body = body.ToMessageBody();
            return mailMessage;

        }
        public async Task<bool> SendEmailAsync(MailData mailData, CancellationToken ct)
        {
            var emailMessage = CreateEmailMessage(mailData);
            using (var smtp = new SmtpClient())
            {
                try
                {
                    if (_settings.UseSSL)
                    {
                        _logger.LogInformation($"The Connection using with UseSSL");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                    }
                    else if (_settings.UseStartTls)
                    {
                        _logger.LogInformation($"The Connection using with StartTls");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls, ct);

                    }

                    await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                    await smtp.SendAsync(emailMessage, ct);
                    _logger.LogInformation($"The Mail sent");
                    return true;
                }
                catch
                {
                    _logger.LogError($"Mail didn't send");
                    return false;
                }
                finally
                {
                    await smtp.DisconnectAsync(true, ct);
                }
            }
        }

        public async Task<bool> SendWithAttachmentsAsync(MailData mailData, CancellationToken ct)
        {
            var emailMessage = CreateEmailMessage(mailData);
            using (var smtp = new SmtpClient())
            {
                try
                {
                    if (_settings.UseSSL)
                    {
                        _logger.LogInformation($"The Connection using with UseSSL");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                    }
                    else if (_settings.UseStartTls)
                    {
                        _logger.LogInformation($"The Connection using with StartTls");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls, ct);

                    }

                    await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                    await smtp.SendAsync(emailMessage, ct);
                    _logger.LogInformation($"The Mail sent");
                    return true;
                }
                catch
                {
                    _logger.LogError("Mail didn't send");
                    return false;
                }
                finally
                {
                    await smtp.DisconnectAsync(true, ct);
                }
            }
        }
        
        public async Task<bool> SendHtmlEmailAsync(MailData mailData, CancellationToken ct)
        {
            var emailMessage = HtmlCreateEmailMessage(mailData);
            using (var smtp = new SmtpClient())
            {
                try
                {
                    if (_settings.UseSSL)
                    {
                        _logger.LogInformation($"The Connection using with UseSSL");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                    }
                    else if (_settings.UseStartTls)
                    {
                        _logger.LogInformation($"The Connection using with StartTls");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls, ct);

                    }

                    await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                    await smtp.SendAsync(emailMessage, ct);
                    _logger.LogInformation($"The Mail sent");
                    return true;
                }
                catch
                {
                    _logger.LogError($"Mail didn't send");
                    return false;
                }
                finally
                {
                    await smtp.DisconnectAsync(true, ct);
                }
            }

        }

        public async Task<bool> SendHtmlWithAttachmentsAsync(MailData mailData, CancellationToken ct)
        {
            var emailMessage = HtmlCreateEmailMessage(mailData);
            using (var smtp = new SmtpClient())
            {
                try
                {
                    if (_settings.UseSSL)
                    {
                        _logger.LogInformation($"The Connection using with UseSSL");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                    }
                    else if (_settings.UseStartTls)
                    {
                        _logger.LogInformation($"The Connection using with StartTls");
                        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls, ct);

                    }

                    await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                    await smtp.SendAsync(emailMessage, ct);
                    _logger.LogInformation($"The Mail sent");
                    return true;
                }
                catch
                {
                    _logger.LogError($"Mail didn't send");
                    return false;
                }
                finally
                {
                    await smtp.DisconnectAsync(true, ct);
                }
            }

        }

    }
}
    