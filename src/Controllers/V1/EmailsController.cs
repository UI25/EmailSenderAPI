using EmailSenderAPI.Models;
using EmailSenderAPI.Services.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using AutoMapper;
using EmailSenderAPI.Models.V1;
using System.Net.Mail;

namespace EmailSenderAPI.Controllers.V1
{

    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailProvider _emailProvider;
        private readonly ILogger<EmailsController> _logger;
        private readonly IMapper _mapper;

        public EmailsController(IEmailProvider emailProvider, ILogger<EmailsController> logger, IMapper mapper)
        {
            _emailProvider = emailProvider;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into EmailController");
            _mapper = mapper;
        }
        [MapToApiVersion("1.0")]
        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail([FromForm] MailDataDto mailDataDto)
        {
            var mailData = new MailData()
            {
                To = mailDataDto.To ?? new List<string>(),
                Subject = mailDataDto.Subject,
                Body = mailDataDto.Body
            };
            
            bool result = await _emailProvider.SendEmailAsync(mailData, new CancellationToken());
            if (result)
            {
                _logger.LogInformation(0, $"Mail has been send ");
                return StatusCode(StatusCodes.Status200OK, "Mail has successfuly been sent.");
            }
            else
            {
                _logger.LogInformation($"Mail hasn't been send");
                return StatusCode(StatusCodes.Status500InternalServerError, "An Error occrured. The Mail count not be sent.");
            }

        }
        [MapToApiVersion("1.0")]
        [HttpPost("SendMailWithAttachments")]
        public async Task<IActionResult> SendMailWithAttacments([FromForm] MailDataWithAttachmentsDto mailDataWithAttachmentsDto)
        {
            var mailData = new MailData()
            {
                To = mailDataWithAttachmentsDto.To ?? new List<string>(),
                Subject = mailDataWithAttachmentsDto.Subject,
                Body = mailDataWithAttachmentsDto.Body,
                Attachments = mailDataWithAttachmentsDto.Attachments ?? new List<IFormFile>()
            };

            bool result = await _emailProvider.SendWithAttachmentsAsync(mailData, new CancellationToken());
            if (result)
            {
                _logger.LogInformation(0, $"Mail has been send ");
                return StatusCode(StatusCodes.Status200OK, "Mail has successfuly been sent.");
            }
            else
            {
                _logger.LogInformation($"Mail hasn't been send");
                return StatusCode(StatusCodes.Status500InternalServerError, "An Error occrured. The Mail count not be sent.");
            }

        }

        [MapToApiVersion("1.0")]
        [HttpPost("HtmlSendMail")]
        public async Task<IActionResult> HtmlSendMail([FromForm] MailDataDto mailDataDto)
        {
            var mailData = new MailData()
            {
                To = mailDataDto.To ?? new List<string>(),
                Subject = mailDataDto.Subject,
                Body = mailDataDto.Body
            };

            bool result = await _emailProvider.SendHtmlEmailAsync(mailData, new CancellationToken());
            if (result)
            {
                _logger.LogInformation(0, $"Mail has been send ");
                return StatusCode(StatusCodes.Status200OK, "Mail has successfuly been sent.");
            }
            else
            {
                _logger.LogInformation($"Mail hasn't been send");
                return StatusCode(StatusCodes.Status500InternalServerError, "An Error occrured. The Mail count not be sent.");
            }

        }

        [MapToApiVersion("1.0")]
        [HttpPost("HtmlSendMailWithAttachments")]
        public async Task<IActionResult> SendHtmlMailWithAttacments([FromForm] MailDataWithAttachmentsDto mailDataWithAttachmentsDto)
        {
            var mailData = new MailData()
            {
                To = mailDataWithAttachmentsDto.To ?? new List<string>(),
                Subject = mailDataWithAttachmentsDto.Subject,
                Body = mailDataWithAttachmentsDto.Body,
                Attachments = mailDataWithAttachmentsDto.Attachments ?? new List<IFormFile>()
            };

            bool result = await _emailProvider.SendHtmlWithAttachmentsAsync(mailData, new CancellationToken());
            if (result)
            {
                _logger.LogInformation(0, $"Mail has been send ");
                return StatusCode(StatusCodes.Status200OK, "Mail has successfuly been sent.");
            }
            else
            {
                _logger.LogInformation($"Mail hasn't been send");
                return StatusCode(StatusCodes.Status500InternalServerError, "An Error occrured. The Mail count not be sent.");
            }

        }


    }
}
