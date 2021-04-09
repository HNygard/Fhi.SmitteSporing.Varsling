using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Microsoft.EntityFrameworkCore;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Optional.Collections;

namespace Fhi.Smittesporing.Varsling.Datalag
{
    public class SmitteVarslingContext : DbContext
    {
        private readonly IArbeidskontekst _arbeidskontekst;

        public SmitteVarslingContext(DbContextOptions<SmitteVarslingContext> options, IArbeidskontekst arbeidskontekst) : base(options)
        {
            _arbeidskontekst = arbeidskontekst;
        }

        public DbSet<Indekspasient> Indekspasienter { get; set; }

        public DbSet<Kommune> Kommuner { get; set; }

        public DbSet<Smittekontakt> Smittekontakt { get; set; }
        public DbSet<SmittekontaktDetaljer> SmittekontaktDetaljer { get; set; }
        public DbSet<SmittekontaktDiagram> SmittekontaktDiagrammer { get; set; }
        public DbSet<SmittekontaktGpsHistogram> SmittekontaktGpsHistogrammer { get; set; }
        public DbSet<SmittekontaktDetaljerHtmlKart> SmittekontaktDetaljerHtmlKart { get; set; }

        public DbSet<Telefon> Telefon { get; set; }

        public DbSet<Applikasjonsinnstilling> Applikasjonsinnstillinger { get; set; }
        public DbSet<SmsVarsel> SmsVarsler { get; set; }

        public DbSet<Innsynlogg> Innsynlogg { get; set; }

        public DbSet<AuditTrail> AuditTrail { get; set; }

        public override int SaveChanges()
        {
            LeggTilAuditTrail();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            LeggTilAuditTrail();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void LeggTilAuditTrail()
        {
            var utfortAv = _arbeidskontekst.HentNavn();

            ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .OfType<IOpprettetAv>()
                .ToList()
                .ForEach(m =>
                {
                    m.OpprettetAv = utfortAv;
                    m.Created = DateTime.Now;
                });

            ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Modified)
                .ToList()
                .ForEach(p =>
                {
                    Audit(p, utfortAv);
                    if (p.Entity is ISistOppdatertAv o)
                    {
                        o.SistOppdatertAv = utfortAv;
                        o.SistOppdatert = DateTime.Now;
                    }
                });
        }

        private void Audit(EntityEntry entry, string utfortAv)
        {
            AuditTrail.AddRange(entry.Properties
                .Where(p => p.IsModified)
                .Select(p =>
                {
                    var auditTrailOptions = p.Metadata.PropertyInfo
                        .GetCustomAttributes(false)
                        .OfType<AuditTrailOptionsAttribute>()
                        .FirstOrNone()
                        .ValueOr(AuditTrailOptionsAttribute.Options);

                    return new AuditTrail
                    {
                        PrimarNokkel = string.Join(",", entry.Metadata
                            .FindPrimaryKey()
                            .Properties
                            .Select(pInfo => pInfo.PropertyInfo.GetValue(entry.Entity)?.ToString())),
                        Modell = entry.Entity.GetType().Name,
                        Egenskap = p.Metadata.Name,
                        GammelVerdi = auditTrailOptions.HentLoggverdi(() => p.OriginalValue?.ToString()),
                        NyVerdi = auditTrailOptions.HentLoggverdi(() => p.CurrentValue?.ToString()),
                        Tidspunkt = DateTime.Now,
                        UtfortAv = utfortAv
                    };
                }));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var indekspasientBuilder = modelBuilder.Entity<Indekspasient>();
            indekspasientBuilder
               .ToTable("Indekspasient", "Msis")
               .HasKey(x => x.IndekspasientId);
            indekspasientBuilder
                .HasMany(s => s.Smittekontakter)
                .WithOne(k => k.Indekspasient)
                .OnDelete(DeleteBehavior.Cascade);

            var telefonBuilder = modelBuilder.Entity<Telefon>();
            telefonBuilder
                .ToTable("Telefon", "Krr")
                .HasKey(telefon => telefon.TelefonId);
            telefonBuilder.HasIndex(x => x.Telefonnummer).IsUnique();
            telefonBuilder
                .HasMany(x => x.IndekspasienterForTelefon)
                .WithOne(x => x.Telefon)
                .OnDelete(DeleteBehavior.Restrict);
            telefonBuilder
                .HasMany(x => x.SmittekontaktForTelefon)
                .WithOne(k => k.Telefon)
                .OnDelete(DeleteBehavior.Restrict);

            var smittekontaktBuilder = modelBuilder.Entity<Smittekontakt>();
            smittekontaktBuilder
                .ToTable("Smittekontakt", "Simula")
                .HasKey(smittekontakt => smittekontakt.SmittekontaktId);
            smittekontaktBuilder
                .HasOne(s => s.SoyleDiagram)
                .WithOne(d => d.Smittekontakt)
                .HasForeignKey<SmittekontaktDiagram>(d => d.SmittekontaktId)
                .OnDelete(DeleteBehavior.Cascade);
            smittekontaktBuilder
                .HasMany(s => s.SmsVarsler)
                .WithOne(v => v.Smittekontakt)
                .HasForeignKey(v => v.SmittekontaktId)
                .OnDelete(DeleteBehavior.Cascade);
            smittekontaktBuilder
                .HasMany(s => s.Detaljer)
                .WithOne(v => v.Smittekontakt)
                .HasForeignKey(v => v.SmittekontaktId)
                .OnDelete(DeleteBehavior.Cascade);

            var smittekontaktDetaljerBuilder = modelBuilder.Entity<SmittekontaktDetaljer>();
            smittekontaktDetaljerBuilder
                .ToTable("SmittekontaktDetaljer", "Simula")
                .HasKey(smittekontakt => smittekontakt.SmittekontaktDetaljerId);
            smittekontaktDetaljerBuilder
                .HasOne(d => d.OppsummertPlotDetaljerHtml)
                .WithOne(k => k.SmittekontaktDetaljer)
                .HasForeignKey<SmittekontaktDetaljerHtmlKart>(x => x.SmittekontaktDetaljerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SmittekontaktDiagram>()
                .ToTable("SmittekontaktDiagram", "Simula")
                .HasKey(x => x.SmittekontaktDiagramId);

            modelBuilder.Entity<SmittekontaktGpsHistogram>()
                .ToTable("SmittekontaktGpsHistogram", "Simula")
                .HasKey(x => x.SmittekontaktGpsHistogramId);

            modelBuilder.Entity<SmittekontaktDetaljerHtmlKart>()
                .ToTable("SmittekontaktDetaljerHtmlKart", "Simula")
                .HasKey(x => x.SmittekontaktDetaljerHtmlKartId);

            var appInnstillingBuilder = modelBuilder.Entity<Applikasjonsinnstilling>();
            appInnstillingBuilder.ToTable("Applikasjonsinnstilling","App").HasKey(i => i.ApplikasjonsinnstillingId);
            appInnstillingBuilder.HasIndex(i => i.Nokkel).IsUnique();

            var smsVarselBuilder = modelBuilder.Entity<SmsVarsel>();
            smsVarselBuilder.ToTable("SmsVarsel","Sms").HasKey(i => i.SmsVarselId);
            smsVarselBuilder.HasIndex(i => i.Referanse).IsUnique();

            var innsynLoggBuilder = modelBuilder.Entity<Innsynlogg>();
            innsynLoggBuilder
                .ToTable("Innsyn","Innsyn")
                .HasKey(innsynlogg => innsynlogg.InnsynloggId);
            innsynLoggBuilder.HasIndex(i => i.Hva);

            modelBuilder.Entity<AuditTrail>()
                .ToTable("AuditTrail", "App")
                .HasKey(x => x.AuditTrailId);

            modelBuilder.Entity<Kommune>()
                .ToTable("Kommune", "App")
                .HasKey(kommune => kommune.KommuneId);
        }

    }
}
