using System;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Test.TestUtils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.DataLag
{
    public class AuditTrailTester
    {
        [Fact]
        public async Task OpprettIndekspasient_SetterCreatedOgOpprettetAv()
        {
            var arbeidskontekts = new Mock<IArbeidskontekst>();
            arbeidskontekts.Setup(x => x.HentNavn()).Returns("Ola");
            var dbContextBuilder = new SmitteVarslingContextBuilder().MedArbeidskontekts(arbeidskontekts.Object);

            await using var targetDb = await dbContextBuilder.BuildAsync();

            targetDb.Indekspasienter.Add(new Indekspasient
            {
                Fodselsnummer = "kryptert:blablabla",
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Status = IndekspasientStatus.Registrert,
                Telefon = new Telefon
                {
                    Telefonnummer = "kryptert:asdasdasd"
                }
            });

            var startDato = DateTime.Now;

            await targetDb.SaveChangesAsync();

            await using var assertDb = await dbContextBuilder.BuildAsync();
            var indekspasient = await assertDb.Indekspasienter
                .Include(i => i.Telefon)
                .FirstOrDefaultAsync();

            indekspasient.OpprettetAv.Should().Be("Ola");
            indekspasient.Created.Should().BeAfter(startDato);

            indekspasient.Telefon.OpprettetAv.Should().Be("Ola");
            indekspasient.Telefon.Created.Should().BeAfter(startDato);
        }

        [Fact]
        public async Task EndreIndekspasient_SetterOppdatertOgOppdatertAv()
        {
            var arbeidskontekts = new Mock<IArbeidskontekst>();
            arbeidskontekts.Setup(x => x.HentNavn()).Returns("Ola");
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedArbeidskontekts(arbeidskontekts.Object)
                .MedDataSeeding(async seedDb =>
                {
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        Fodselsnummer = "kryptert:blablabla",
                        Provedato = DateTime.Now.AddDays(-3),
                        Opprettettidspunkt = DateTime.Now.AddDays(-1),
                        Status = IndekspasientStatus.Registrert,
                        Telefon = new Telefon
                        {
                            Telefonnummer = "kryptert:asdasdasd"
                        }
                    });
                    await seedDb.SaveChangesAsync();
                });

            await using var targetDb = await dbContextBuilder.BuildAsync();

            var startDato = DateTime.Now;

            var indekspasient = await targetDb.Indekspasienter.FirstOrDefaultAsync();

            indekspasient.Status = IndekspasientStatus.IkkeSmitteKontakt;

            await targetDb.SaveChangesAsync();

            await using var assertDb = await dbContextBuilder.BuildAsync();
            var indekspasientAssert = await assertDb.Indekspasienter
                .Include(i => i.Telefon)
                .FirstOrDefaultAsync();

            indekspasientAssert.SistOppdatertAv.Should().Be("Ola");
            indekspasientAssert.SistOppdatert.Should().BeAfter(startDato);
        }

        [Fact]
        public async Task EndreIndekspasient_LagerAuditTrail()
        {
            var arbeidskontekts = new Mock<IArbeidskontekst>();
            arbeidskontekts.Setup(x => x.HentNavn()).Returns("Ola");
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedArbeidskontekts(arbeidskontekts.Object)
                .MedDataSeeding(async seedDb =>
                {
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        Fodselsnummer = "kryptert:blablabla",
                        Provedato = DateTime.Now.AddDays(-3),
                        Opprettettidspunkt = DateTime.Now.AddDays(-1),
                        Status = IndekspasientStatus.Registrert,
                        Telefon = new Telefon
                        {
                            Telefonnummer = "kryptert:asdasdasd"
                        }
                    });
                    await seedDb.SaveChangesAsync();
                });

            await using var targetDb = await dbContextBuilder.BuildAsync();

            var indekspasient = await targetDb.Indekspasienter.FirstOrDefaultAsync();

            indekspasient.Status = IndekspasientStatus.IkkeSmitteKontakt;
            indekspasient.Fodselsnummer = "kryptert:nyverdi";

            await targetDb.SaveChangesAsync();

            await using var assertDb = await dbContextBuilder.BuildAsync();
            var auditTrail = await assertDb.AuditTrail.ToListAsync();

            auditTrail.Count.Should().Be(2);
            auditTrail.Should().Contain(a =>
                a.PrimarNokkel == indekspasient.IndekspasientId.ToString() &&
                a.UtfortAv == "Ola" &&
                a.Modell == nameof(Indekspasient) &&
                a.Egenskap == nameof(Indekspasient.Fodselsnummer) &&
                a.GammelVerdi == "<Sensitiv>" &&
                a.NyVerdi == "<Sensitiv>"
            );
            auditTrail.Should().Contain(a =>
                a.PrimarNokkel == indekspasient.IndekspasientId.ToString() &&
                a.UtfortAv == "Ola" &&
                a.Modell == nameof(Indekspasient) &&
                a.Egenskap == nameof(Indekspasient.Status) &&
                a.GammelVerdi == IndekspasientStatus.Registrert.ToString() &&
                a.NyVerdi == IndekspasientStatus.IkkeSmitteKontakt.ToString()
            );
        }
    }
}