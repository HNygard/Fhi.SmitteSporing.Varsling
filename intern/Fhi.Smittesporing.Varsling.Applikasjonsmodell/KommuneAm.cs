namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell
{
    public class KommuneAm
    {
        public int KommuneId { get; set; }
        public string Navn { get; set; }
        public string KommuneNr { get; set; }
        public string SmsFletteinfo { get; set; }

        public class OppdaterSmsFletteinfoCommand
        {
            public string SmsFletteinfo { get; set; }
        }
    }
}