using System;

namespace Fhi.Smittesporing.Helsenorge.Api.Models
{
	public class InnsynLoggHn
    {
		public string Navn { get; set; }
		public string Organisasjon { get; set; }
		public string Formal { get; set; }
		public DateTime Dato { get; set; }
	}
}
