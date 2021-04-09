using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IKontaktInfoFacade
    {
        Task<KontaktinformasjonReponse> HentPersoner(IEnumerable<string> fnr);
    }

    public class KontaktinformasjonReponse
    {
        public StatusKoder Status { get; set; }
        public List<Kontaktinformasjon> Kontaktinformasjon { get; set; }
        public FeilInfo FeilInfo { get; set; }
    }

    public enum StatusKoder
    {
        Ok = 0,
        IkkeKontaktOrdinaeSone,
        IkkeKontaktDifi,
        SertifikatFeil,
        AnnenFeil
    }

    public class FeilInfo
    {
        public List<string> InfoTekst { get; set; }
    }

    public class Kontaktinformasjon
    {
        public string Fnr { get; set; }
        public string Mobil { get; set; }
        public bool Reservasjon { get; set; } = false;
        public PersonStatus Status { get; set; } = PersonStatus.IkkeRegistrert;
    }

    public enum PersonStatus
    {
        Aktiv,
        Slettet,
        IkkeRegistrert
    }
}
