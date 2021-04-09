using Fhi.Smittesporing.Varsling.Felles.Domene;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn
{
    public class InnsynFilter : IPagedQuery
    {
        public Option<string> Fodselsnummer { get; set; }
        public Option<string> Telefonnummer { get; set; }
        public Option<int> Sideindeks { get; set; }
        public Option<int> Sideantall { get; set; }
    }
}
