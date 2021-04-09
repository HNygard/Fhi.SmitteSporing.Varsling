using System;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Preg
{
    public class PregPerson
    {
        public string Identifikator { get; set; }
        public Option<DateTime> Fodselsdato { get; set; }
        public bool HarHemmeligAdresse { get; set; }
    }
}