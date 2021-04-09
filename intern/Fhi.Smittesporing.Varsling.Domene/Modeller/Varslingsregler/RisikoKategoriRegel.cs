using System.Linq;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler
{
    /// <summary>
    /// Kontakt må ha en risikokategori som er satt opp som gyldig for varsling
    /// </summary>
    public class RisikoKategoriRegel : IVarslingsregel
    {
        private readonly Konfig _konfig;

        public RisikoKategoriRegel(IOptions<Konfig> konfig)
        {
            _konfig = konfig.Value;
        }

        public bool KanVarsles(Smittekontakt kontakt)
        {
            return _konfig.RisikoKategorierSomVarsles == null ||
                   _konfig.RisikoKategorierSomVarsles.Contains(kontakt.Risikokategori);
        }

        public string Navn => "Varsle kun over risikoterskel";
        public string Beskrivelse => _konfig.RisikoKategorierSomVarsles == null
            ? "Varsel sendes uavhengig av risikokategorisering"
            : $"Risikokategoriseri må være en av: {string.Join(", ", _konfig.RisikoKategorierSomVarsles)}";

        public class Konfig
        {
            public string[] RisikoKategorierSomVarsles { get; set; } = null;
        }
    }
}