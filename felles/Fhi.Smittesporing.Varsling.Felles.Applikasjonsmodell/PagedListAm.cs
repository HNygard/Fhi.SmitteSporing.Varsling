using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell
{
    public class PagedListAm<T>
    {
        public int TotaltAntall { get; set; }
        public int Sideindeks { get; set; }
        public int Sideantall { get; set; }
        public int AntallSider { get; set; }
        public IEnumerable<T> Resultater { get; set; }
    }
}