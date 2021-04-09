using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Datalag.Repositories;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Test.TestUtils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.DataLag
{
    public class ApplikasjonsinnstillingRepositoryTester
    {
        [Fact]
        public async Task HentInnstilling_TomTabell_ReturnererNone()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder();

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<ApplikasjonsinnstillingRepository>();

            var verdi = await target.HentInnstilling<int>("test");

            verdi.Should().Be(Option.None<int>());
        }

        [Fact]
        public async Task HentInnstilling_VerdiFinnes_ReturnererRiktig()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Applikasjonsinnstillinger.Add(new Applikasjonsinnstilling
                    {
                        ApplikasjonsinnstillingId = 1,
                        Nokkel = "ikke-test",
                        Verdi = "-1"
                    });
                    seedDb.Applikasjonsinnstillinger.Add(new Applikasjonsinnstilling
                    {
                        ApplikasjonsinnstillingId = 2,
                        Nokkel = "test",
                        Verdi = "42"
                    });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<ApplikasjonsinnstillingRepository>();

            var verdi = await target.HentInnstilling<int>("test");

            verdi.Should().Be(42.Some());
        }

        [Fact]
        public async Task SettInnstilling_NyVerdi_OppretterNyRad()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Applikasjonsinnstillinger.Add(new Applikasjonsinnstilling
                    {
                        ApplikasjonsinnstillingId = 2,
                        Nokkel = "test",
                        Verdi = "42"
                    });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<ApplikasjonsinnstillingRepository>();

            await target.SettInnstilling("test", 42);

            var rader = await db.Applikasjonsinnstillinger.ToListAsync();

            rader.Count.Should().Be(1);
            rader.Should().Contain(x => x.ApplikasjonsinnstillingId == 2 && x.Nokkel == "test" && x.Verdi == "42");
        }


        [Fact]
        public async Task SettInnstilling_FinnesMedGammelVerdi_OppdatererEksisternde()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder();

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<ApplikasjonsinnstillingRepository>();

            await target.SettInnstilling("test", 42);

            var rader = await db.Applikasjonsinnstillinger.ToListAsync();

            rader.Count.Should().Be(1);
            rader.Should().Contain(x => x.Nokkel == "test" && x.Verdi == "42");
        }
    }
}