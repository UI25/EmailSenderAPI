using AutoMapper;
using EmailSenderAPI.Models.V1;
using MimeKit.Cryptography;
using NLog.LayoutRenderers;

namespace EmailSenderAPI.Models
{
    public class MailProfile : Profile
    {
        public MailProfile() 
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MailData, MailDataDto>().ReverseMap();
                cfg.CreateMap<MailDataDto, MailData>().ReverseMap();
          
            });
            IMapper mapper = config.CreateMapper();
        }
    }
}
