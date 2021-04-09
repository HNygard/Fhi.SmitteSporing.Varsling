using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Intern.MockControllers
{
    [ApiController]
    [Route("mockapi/[controller]")]
    [ApiExplorerSettings(GroupName = "mocks")]
    public class KontaktInfoMockController : ControllerBase
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        private static readonly string[] Prefix = { "", "", "", "0047", "+47", "0048", "+48" };

        [HttpPost("hentPersoner")]
        public ActionResult<KontaktinformasjonReponse> HentPersoner([FromBody] List<string> fnr, string key)
        {
            var kontaktinformasjonListe = new List<Kontaktinformasjon>();

            foreach (var fodselnummer in fnr)
            {
                if (fodselnummer.EndsWith("5") || fodselnummer.EndsWith("6"))
                {
                    kontaktinformasjonListe.Add(new Kontaktinformasjon
                    {
                        Status = PersonStatus.IkkeRegistrert,
                        Fnr = fodselnummer,
                    });
                    continue;
                }

                var mobil = Rand.NextDouble() < 0.1
                    ? null
                    : Prefix[Rand.Next(Prefix.Length)] + Rand.Next(9000000, 99999999);

                kontaktinformasjonListe.Add(new Kontaktinformasjon
                {
                    Status = PersonStatus.Aktiv,
                    Fnr = fodselnummer,
                    Mobil = mobil,
                    Epost = "dummy@email.com"
                });
            }

            var kontaktinformasjonReponse = new KontaktinformasjonReponse()
            {
                Status = StatusKoder.Ok,
                Kontaktinformasjon = kontaktinformasjonListe
            };
            return Ok(kontaktinformasjonReponse);
        } // HentPersoner



        //kopi fra Fhi.KontaktInfo
        //kun til bruk i denne Controller

        public class KontaktinformasjonReponse
        {
            public StatusKoder Status { get; set; }
            public List<Kontaktinformasjon> Kontaktinformasjon { get; set; }
            public FeilInfo FeilInfo { get; set; }
            //Felt som trengs for Digital postkasse og print sertifikat:
            public PrintSertifikat PrintSertifikat { get; set; }
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
            public string Epost { get; set; }
            public bool Reservasjon { get; set; } = false;
            public PersonStatus Status { get; set; } = PersonStatus.IkkeRegistrert;

            //Felter som trengs for Digital postkasse:
            public DigitalPostadresse DigitalPostadresse { get; set; }
            public X509Certificate Sertifikat { get; set; }

        }

        public class PrintSertifikat
        {
            public string LeverandørAdresse { get; set; }
            public X509Certificate Sertifikat { get; set; }
        }

        public class DigitalPostadresse
        {
            public string Postkasseadresse { get; set; }
            public string LeverandørAdresse { get; set; }
        }

        public enum PersonStatus
        {
            Aktiv,
            Slettet,
            IkkeRegistrert
        }

    }

}
