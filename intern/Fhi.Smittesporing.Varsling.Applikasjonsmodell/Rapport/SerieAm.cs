using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport
{
    public class SerieAm
    {
        public SerieAm(string label, List<int> data)
        {
            Label = label;
            Data = data;
        }
        public List<int> Data { get; set; }

        public string Label { get; set; }
    }
}
