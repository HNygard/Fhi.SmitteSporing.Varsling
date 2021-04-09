using AutoMapper;
using Fhi.Smittesporing.Helsenorge.Api.Models;
using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public class HelseNorgeApiMapperProfil : Profile
    {
        public HelseNorgeApiMapperProfil()
        {
            CreateMap<InnsynLoggHelsenorgeAm, InnsynLoggHn>();

            CreateMap<InnsynHelsenorgeAm, InnsynHn>();
            CreateMap<InnsynHelsenorgeSmittekontaktAm, SmitteKontaktHn>();
            CreateMap<InnsynHelsenorgeSmsvarselAm, SmsVarselHn>();

            CreateMap<InnsynHendelserHelsenorgeAm, InnsynHendelserHn>();
            CreateMap<InnsynHendelseHelseNorgeAm, InnsynHendelseHn>();
        }
    }
}
