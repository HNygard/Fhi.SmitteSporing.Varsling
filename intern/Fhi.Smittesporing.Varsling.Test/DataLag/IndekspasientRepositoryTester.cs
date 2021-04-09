using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Datalag.Repositories;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Test.TestUtils;
using FluentAssertions;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.DataLag
{
    public class IndekspasientRepositoryTester
    {
        [Fact]
        public async Task HentEldsteGodkjentMenIkkeVarslet_GittGodkjentSmittetilfelle_ReturnererTilfellet()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        IndekspasientId = 41,
                        Varslingsstatus = Varslingsstatus.TilGodkjenning
                    });
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        IndekspasientId = 42,
                        Varslingsstatus = Varslingsstatus.Godkjent
                    });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<IndekspasientRepository>();

            var godkjentSmittetilfelle = await target.HentEldsteGodkjentMenIkkeVarslet();

            godkjentSmittetilfelle.HasValue.Should().Be(true);
            godkjentSmittetilfelle
                .Map(x => x.IndekspasientId)
                .ValueOr(-1)
                .Should().Be(42);
        }

        [Theory]
        [InlineData(Varslingsstatus.TilGodkjenning)]
        [InlineData(Varslingsstatus.Klargjort)]
        [InlineData(Varslingsstatus.Ferdig)]
        public async Task HentEldsteGodkjentMenIkkeVarslet_KunIkkeGodkjent_ReturnererNone(Varslingsstatus status)
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedNavneSuffix(status.ToString())
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        IndekspasientId = 42,
                        Varslingsstatus = status
                    });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<IndekspasientRepository>();

            var resultat = await target.HentEldsteGodkjentMenIkkeVarslet();

            resultat.HasValue.Should().Be(false);
        }

        [Fact]
        public async Task HentMedAntall_GittManglerKontaktinfoFilter_ReturnererManglerKontaktInfo()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        IndekspasientId = 42,
                        Status = IndekspasientStatus.KontaktInfoMangler
                    });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<IndekspasientRepository>();

            var resultat = await target.HentMedAntall(new Indekspasient.Filter
            {
                ManglerKontaktinfo = true.Some()
            });

            resultat.TotaltAntall.Should().Be(1);
        }

        [Theory]
        [InlineData(IndekspasientStatus.Slettet)]
        [InlineData(IndekspasientStatus.IkkeSmitteKontakt)]
        [InlineData(IndekspasientStatus.SmitteKontakt)]
        [InlineData(IndekspasientStatus.Registrert)]
        public async Task HentMedAntall_GittManglerKontaktinfoFilter_ReturnererIkkeAndreStatuser(IndekspasientStatus status)
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedNavneSuffix(status.ToString())
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Indekspasienter.Add(new Indekspasient
                    {
                        IndekspasientId = 42,
                        Status = status
                    });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<IndekspasientRepository>();

            var resultat = await target.HentMedAntall(new Indekspasient.Filter
            {
                ManglerKontaktinfo = true.Some()
            });

            resultat.TotaltAntall.Should().Be(0);
        }

        [Fact]
        public async Task HentRapport_GittFilter_TarKunMedRiktigPasient()
        {
            var dbContextBuilder = new SmitteVarslingContextBuilder()
                .MedDataSeeding(seedDb =>
                {
                    seedDb.Indekspasienter.AddRange(
                        new Indekspasient
                        {
                            IndekspasientId = 1,
                            Status = IndekspasientStatus.SmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddDays(-4).AddHours(1),
                            Smittekontakter = new List<Smittekontakt>
                            {
                                new Smittekontakt
                                {
                                    SmittekontaktId = 1
                                }
                            }
                        },
                        new Indekspasient
                        {
                            IndekspasientId = 2,
                            Status = IndekspasientStatus.SmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddDays(-3).AddHours(1),
                            Smittekontakter = new List<Smittekontakt>
                            {
                                new Smittekontakt
                                {
                                    SmittekontaktId = 2
                                },
                                new Smittekontakt
                                {
                                    SmittekontaktId = 3
                                }
                            }
                        },
                        new Indekspasient
                        {
                            IndekspasientId = 3,
                            Status = IndekspasientStatus.SmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddDays(-2).AddHours(1),
                            Smittekontakter = new List<Smittekontakt>
                            {
                                new Smittekontakt
                                {
                                    SmittekontaktId = 4
                                }
                            }
                        },
                        new Indekspasient
                        {
                            IndekspasientId = 4,
                            Status = IndekspasientStatus.IkkeSmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddDays(-2).AddHours(1)
                        },
                        new Indekspasient
                        {
                            IndekspasientId = 5,
                            Status = IndekspasientStatus.IkkeSmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddDays(-1).AddHours(1)
                        },
                        new Indekspasient
                        {
                            IndekspasientId = 6,
                            Status = IndekspasientStatus.IkkeSmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddDays(-1).AddHours(1)
                        },
                        new Indekspasient
                        {
                            IndekspasientId = 7,
                            Status = IndekspasientStatus.IkkeSmitteKontakt,
                            Opprettettidspunkt = DateTime.Today.AddHours(1)
                        });
                    return seedDb.SaveChangesAsync();
                });

            await using var db = await dbContextBuilder.BuildAsync();

            var automocker = new AutoMocker();
            automocker.Use(db);

            var target = automocker.CreateInstance<IndekspasientRepository>();

            var resultat = await target.HentRapport(new Indekspasient.Filter
            {
                FraOgMed = DateTime.Today.AddDays(-3).Some(),
                TilOgMed = DateTime.Today.AddSeconds(-1).Some()
            });

            resultat.AntallSisteDag.Should().Be(2);

            resultat.ChartData.Series
                .First(s => s.Label == "Nye indekspasienter med kontakt")
                .Data.Should().Equal(1, 1, 0);

            resultat.ChartData.Series
                .First(s => s.Label == "Nye indekspasienter uten kontakt")
                .Data.Should().Equal(0, 1, 2);

            resultat.ChartData.Series
                .First(s => s.Label == "Nye smittekontakter")
                .Data.Should().Equal(2, 1, 0);
        }
    }
}