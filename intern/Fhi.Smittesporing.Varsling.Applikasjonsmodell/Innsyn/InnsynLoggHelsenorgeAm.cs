using System;
using System.Collections.Generic;
using System.Text;

namespace Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn
{
    public class InnsynLoggHelsenorgeAm
    {
        public string Navn { get; set; }
        public string Organisasjon { get; set; }
        public string Formal { get; set; }
        public DateTime Dato { get; set; }
    }

    public class InnsynLoggHelsenorgeAmEqualityComparer : IEqualityComparer<InnsynLoggHelsenorgeAm>
    {
        private readonly TimeSpan Delta = TimeSpan.FromSeconds(60 * 5);

        public bool Equals(InnsynLoggHelsenorgeAm x, InnsynLoggHelsenorgeAm y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null && y != null)
            {
                return false;
            }

            if (x == null && y != null)
            {
                return false;
            }

            var rest = 
                x.Formal == y.Formal &&
                x.Navn == y.Navn &&
                x.Organisasjon == y.Organisasjon;

            return rest && (Math.Abs(x.Dato.Subtract(y.Dato).TotalSeconds) <= Delta.TotalSeconds);
        }

        public int GetHashCode(InnsynLoggHelsenorgeAm obj)
        {
            return GetHashCode();
        }
    }
}
