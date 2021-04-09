using System;
using System.Linq;
using AutoMapper;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene
{
    public class DomeneMapperprofil : Profile
    {
        public DomeneMapperprofil()
        {
            CreateMap<Smittekontakt, SmittekontaktListemodellAm>(MemberList.None);
            CreateMap<SmittekontaktOgSisteKontaktdato, SmittekontaktListemodellAm>()
                .IncludeMembers(src => src.Smittekontakt);

            CreateMap<Smittekontakt, SmittekontaktAm>(MemberList.None);
            CreateMap<SmittekontaktMedDetaljer, SmittekontaktAm>()
                .IncludeMembers(src => src.Smittekontakt)
                .ForMember(dst => dst.Detaljer, opt => opt.MapFrom(
                    src => src.Detaljer.OrderByDescending(d => d.SmittekontaktDetaljer.Dato)));
            CreateMap<SmittekontaktDetaljer, SmittekontaktDetaljerAm>(MemberList.None);
            CreateMap<SmittekontaktDetaljerMedKartinfo, SmittekontaktDetaljerAm>()
                .IncludeMembers(src => src.SmittekontaktDetaljer);
            CreateMap<SmsVarsel, SmsVarselAm>()
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dst => dst.Oppdateringer, opt => opt.Ignore())
                .ForMember(dst => dst.SisteEksterneHendelsestidspunkt, opt => opt.MapFrom(src => ForceLocalTime(src.SisteEksterneHendelsestidspunkt)));

            CreateMap<Indekspasient, IndekspasientAm>()
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dst => dst.Varslingsstatus, opt => opt.MapFrom(src => src.Varslingsstatus.ToString()));
            CreateMap<IndekspasientOgAntall, IndekspasientMedAntallAm>()
                .IncludeMembers(src => src.Indekspasient);
            CreateMap<Indekspasient, IndekspasientMedAntallAm>(MemberList.None)
                .IncludeBase<Indekspasient, IndekspasientAm>();

            CreateMap<SmsFletteinnstillinger, SmsFletteinnstillingerAm>();
            CreateMap<SmsFletteinnstillingerAm, SmsFletteinnstillinger>();

            CreateMap<IndekspasientRapport, IndekspasientRapportAm>();
            CreateMap<ChartData, ChartDataAm>();
            CreateMap<Periode, PeriodeAm>();

            CreateMap<Kommune, KommuneAm>();
            CreateMap<SmsVarselMal, SmsVarselMalAm>();
            CreateMap<SmsFlettefelt, SmsFlettefeltAm>();
            CreateMap<SmsStatusoppdatering, SmsStatusoppdateringAm>()
                .ForMember(dst => dst.GjeldeneStatus, opt => opt.MapFrom(src => src.GjeldeneStatus.ToString()));
            CreateMap<SmsTilgang, SmsTilgangAm>();

            CreateMap<IndekspasientAm.Filter, Indekspasient.Filter>()
                .ForMember(dst => dst.IndekspasientId, opt => opt.Ignore())
                .ForMember(dst => dst.VisSkjulteStatuser, opt => opt.Ignore());
            CreateMap<SmittekontaktAm.Filter, Smittekontakt.Filter>();

            CreateMap<SimulaGpsData, InnsynSimulaGpsDataAm>()
                .ForMember(dst => dst.FraTidspunkt, opt => opt.MapFrom(src => ForceLocalTime(src.FraTidspunkt)))
                .ForMember(dst => dst.TilTidspunkt, opt => opt.MapFrom(src => ForceLocalTime(src.TilTidspunkt)));
            CreateMap<SimulaDataBruk, InnsynSimulaDatabrukAm>()
                .ForMember(dst => dst.Tidspunkt, opt => opt.MapFrom(src => ForceLocalTime(src.Tidspunkt)));
            CreateMap<Innsynlogg, InnsynLoggAm>();
            CreateMap<Smittekontakt, InnsynSmittekontaktAm>()
                .ForMember(dst => dst.Risikokategori, opt => opt.MapFrom(src => RisikokategoriEngelskTilNorsk(src.Risikokategori)));
            CreateMap<Indekspasient, InnsynIndekspasientAm>()
                .ForMember(dst => dst.Kommune, opt => opt.MapFrom(source => source.Kommune.Navn));
            CreateMap<SmsVarsel, InnsynSmsVarselAm>()
                .ForMember(dst => dst.Verifiseringskode, opt => opt.MapFrom(src => src.Smittekontakt.Verifiseringskode))
                .ForMember(dst => dst.SisteEksterneHendelsestidspunkt, opt => opt.MapFrom(src => ForceLocalTime(src.SisteEksterneHendelsestidspunkt)));
            CreateMap<InnsynFilterAm, InnsynFilter>();

            CreateMap<int?, Option<int>>().ConvertUsing<OptionNullableConverter<int>>();
            CreateMap<Option<int>, int?>().ConvertUsing<OptionNullableConverter<int>>();
            CreateMap<string, Option<string>>().ConvertUsing<OptionConverter<string>>();
            CreateMap<Option<string>, string>().ConvertUsing<OptionConverter<string>>();
            CreateMap<Option<Guid>, Guid?>().ConvertUsing<OptionNullableConverter<Guid>>();
            CreateMap<bool?, Option<bool>>().ConvertUsing<OptionNullableConverter<bool>>();
            CreateMap<DateTime?, Option<DateTime>>().ConvertUsing<OptionNullableConverter<DateTime>>();


        }

        private static DateTime? ForceLocalTime(DateTime? input)
        {
            if (input.HasValue)
            {
                DateTime convertedDate = DateTime.SpecifyKind(input.Value, DateTimeKind.Utc);

                return convertedDate.ToLocalTime();
            }

            return null;
        }

        private string RisikokategoriEngelskTilNorsk(string input)
        {
            switch(input)
            {
                case "low": return "lav";
                case "medium": return "middels";
                case "high": return "høy";
            }

            return string.Empty;
        }
    }

    public class OptionConverter<T> : ITypeConverter<Option<T>, T>, ITypeConverter<T, Option<T>> where T : class
    {
        public T Convert(Option<T> source, T destination, ResolutionContext context) => source.ValueOr((T)null);
        public Option<T> Convert(T source, Option<T> destination, ResolutionContext context) => source.SomeNotNull();
    }

    public class OptionNullableConverter<T> : ITypeConverter<Option<T>, T?>, ITypeConverter<T?, Option<T>> where T : struct
    {
        public T? Convert(Option<T> source, T? destination, ResolutionContext context) => source.Match(x => (T?)x, () => null);
        public Option<T> Convert(T? source, Option<T> destination, ResolutionContext context) => source.ToOption();
    }
}