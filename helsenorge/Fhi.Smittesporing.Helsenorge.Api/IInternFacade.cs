using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public interface IInternFacade
    {
        Task<IEnumerable<InnsynLoggHelsenorgeAm>> HentInnsynlogg(HelsenorgeToken token);
        Task<InnsynHelsenorgeAm> HentInnsynHelsenorge(HelsenorgeToken token);
        Task<InnsynHendelserHelsenorgeAm> HentHelsenorgeHendelser(HelsenorgeToken token);
    }

    public class InternFacade : IInternFacade
    {
        private readonly Varslingklient _klient;

        public InternFacade(Varslingklient klient)
        {
            _klient = klient;
        }

        public async Task<InnsynHendelserHelsenorgeAm> HentHelsenorgeHendelser(HelsenorgeToken token)
        {
            return await _klient.HentHelsenorgeHendelser(token);
        }

        public async Task<InnsynHelsenorgeAm> HentInnsynHelsenorge(HelsenorgeToken token)
        {
            return await _klient.HentInnsynHelsenorge(token);
        }

        public async Task<IEnumerable<InnsynLoggHelsenorgeAm>> HentInnsynlogg(HelsenorgeToken token)
        {
            return await _klient.HentInnsynlogg(token);
        }
    }
}
