using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Bakgrunnsjobber
{
    public class FjerntUtgatteDataJobbTester : BakgrunnsjobbTestbase
    {
        private readonly Mock<ILogger<FjerntUtgatteDataJobb>> _logger;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOptions<FjerntUtgatteDataJobb.Konfig>> _konfig;

        public FjerntUtgatteDataJobbTester()
        {
            _logger = new Mock<ILogger<FjerntUtgatteDataJobb>>();
            _mediator = new Mock<IMediator>();
            _konfig = new Mock<IOptions<FjerntUtgatteDataJobb.Konfig>>();

            //  mock mediator HentManglerKontaktinfo
            _mediator.Setup(m => m.Send(It.IsAny<FjernUtgatteIndekspasienter.Command>(), It.IsAny<CancellationToken>()))
                    .Returns((FjernUtgatteIndekspasienter.Command request, CancellationToken cancellationToken) =>
                    {
                        var handler = new FjernUtgatteIndekspasienter.Handler(IndekspasientRepository);
                        return handler.Handle(request, cancellationToken);
                    });
        }

        /// <summary>
        ///  Slette alt som er opprettet  
        ///  Sletter alt med 'Created' tidligere enn beregnet UtgattTidspunkt    
        ///  /// </summary>
        [Fact]
        [Trait("HostService", "FjernUtgatteIndekspasienter")]
        public async Task FjernAlle_Test()
        {
            //sett uttgåttTidspunkt til 10 tidligere
            _konfig.Setup(x => x.Value).Returns(new FjerntUtgatteDataJobb.Konfig
            {
                SlettDataEldreEnn = TimeSpan.FromDays(10)
            });

            //sett 'OpprettetTidspunkt' tidligere  enn 'Utgåttdato'
            InsertSmitteTilfelleHelper("12345678901", "12345678", (s) => { s.Opprettettidspunkt = DateTime.Now.AddDays(-10).AddMinutes(-1); });
            InsertSmitteTilfelleHelper("12345678902", "12345679", (s) => { s.Opprettettidspunkt = DateTime.Now.AddDays(-10).AddMinutes(-1); });
       
            var target = new FjerntUtgatteDataJobb(_konfig.Object, _logger.Object, _mediator.Object);

            var fantArbeid = await target.UtforJobb(new CancellationToken());

            Assert.True(fantArbeid);
            Assert.Equal(0, DbContext.Indekspasienter.Count());
            Assert.Equal(0, DbContext.Smittekontakt.Count());
            Assert.Equal(0, DbContext.SmittekontaktDetaljer.Count());
            Assert.Equal(0, DbContext.SmsVarsler.Count());
            Assert.Equal(0, DbContext.Telefon.Count());
        }

        /// <summary>
        ///  Ikke slette noe  
        ///  /// </summary>
        [Fact]
        [Trait("HostService", "FjernUtgatteIndekspasienter")]
        public async Task FjernIngenting_Test()
        {
            //sett uttgåttTidspunkt til 10 tidligere
            _konfig.Setup(x => x.Value).Returns(new FjerntUtgatteDataJobb.Konfig
            {
                SlettDataEldreEnn = TimeSpan.FromDays(10)
            });

            //sett 'Created' etter 'Utgåttdato'
            InsertSmitteTilfelleHelper("12345678901", "12345678", (s) => { s.Opprettettidspunkt = DateTime.Now.AddMinutes(-5); });
            InsertSmitteTilfelleHelper("12345678902", "12345679", (s) => { s.Opprettettidspunkt = DateTime.Now.AddMinutes(-5); });

            var antallSmittetilfelle = DbContext.Indekspasienter.Count();
            var antallSmittekontakt = DbContext.Smittekontakt.Count();
            var antallSmittekontaktDetaljer = DbContext.SmittekontaktDetaljer.Count();
            var antallSmsVarsler = DbContext.SmsVarsler.Count();
            var antallTelefon = DbContext.Telefon.Count();

            var target = new FjerntUtgatteDataJobb(_konfig.Object, _logger.Object, _mediator.Object);

            var fjernet = await target.UtforJobb(new CancellationToken());

            Assert.False(fjernet);
            Assert.Equal(antallSmittetilfelle, DbContext.Indekspasienter.Count());
            Assert.Equal(antallSmittekontakt, DbContext.Smittekontakt.Count());
            Assert.Equal(antallSmittekontaktDetaljer, DbContext.SmittekontaktDetaljer.Count());
            Assert.Equal(antallSmsVarsler, DbContext.SmsVarsler.Count());
            Assert.Equal(antallTelefon, DbContext.Telefon.Count());
        }

        /// <summary>
        ///  slett bare utgåtte  
        ///  /// </summary>
        [Fact]
        [Trait("HostService", "FjernUtgatteIndekspasienter")]
        public async Task FjernBareUtgatte_Test()
        {
            //sett uttgåttTidspunkt til 10 tidligere
            _konfig.Setup(x => x.Value).Returns(new FjerntUtgatteDataJobb.Konfig
            {
                SlettDataEldreEnn = TimeSpan.FromDays(10)
            });

            //sett 'Created' etter 'Utgåttdato'
            InsertSmitteTilfelleHelper("12345678901", "12345678", (s) => { s.Opprettettidspunkt = DateTime.Now.AddDays(-10).AddMinutes(-5); }); //utgått
            InsertSmitteTilfelleHelper("12345678902", "12345679", (s) => { s.Opprettettidspunkt = DateTime.Now.AddDays(-10).AddMinutes(5); }); //avktiv

            var antallSmittetilfelle = DbContext.Indekspasienter.Count();
            var antallSmittekontakt = DbContext.Smittekontakt.Count();
            var antallSmittekontaktDetaljer = DbContext.SmittekontaktDetaljer.Count();
            var antallSmsVarsler = DbContext.SmsVarsler.Count();
            var antallTelefon = DbContext.Telefon.Count();

            var target = new FjerntUtgatteDataJobb(_konfig.Object, _logger.Object, _mediator.Object);

            var fjernet = await target.UtforJobb(new CancellationToken());

            Assert.True(fjernet);

            //halvpart skal være fjernet
            Assert.Equal(antallSmittetilfelle/2, DbContext.Indekspasienter.Count());
            Assert.Equal(antallSmittekontakt/2, DbContext.Smittekontakt.Count());
            Assert.Equal(antallSmittekontaktDetaljer/2, DbContext.SmittekontaktDetaljer.Count());
            Assert.Equal(antallSmsVarsler/2, DbContext.SmsVarsler.Count());
            Assert.Equal(antallTelefon/2, DbContext.Telefon.Count());

        }

        private void InsertSmitteTilfelleHelper(string fodselnummer, string telefonnummer, Action<Indekspasient> action)
        {
            var smitteTilfelle = new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Fodselsnummer = fodselnummer,
                Telefon = string.IsNullOrEmpty(telefonnummer)
                    ? null
                    : new Telefon
                    {
                        Telefonnummer = telefonnummer
                    }
            };

            action(smitteTilfelle);

            DbContext.Add(smitteTilfelle);

            var smitteKontakt = new Smittekontakt
            {
                Detaljer = new List<SmittekontaktDetaljer>
                {
                    new SmittekontaktDetaljer
                    {
                        Dato = smitteTilfelle.Opprettettidspunkt
                    },
                    new SmittekontaktDetaljer
                    {
                        Dato = smitteTilfelle.Opprettettidspunkt
                    }
                },
                Indekspasient = smitteTilfelle
            };

            DbContext.Add(smitteKontakt);

            var smsVarsel = new SmsVarsel
            {
                Smittekontakt = smitteKontakt
            };

            DbContext.Add(smsVarsel);

            DbContext.SaveChanges();
        }
    }
}