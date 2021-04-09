using System;
using System.Collections.Generic;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class InnsynloggRapport
    {
        public InnsynloggRapport()
        {
            Generertdato = DateTime.Now;
            Innsyn = Enumerable.Empty<Line>();
        }

        public DateTime Generertdato { get; }

        public IEnumerable<Line> Innsyn { get; set; }

        public class Line
        {
            public string Who { get; set; }
            public DateTime When { get; set; }
            public string Why { get; set; }
        }
    }
}
