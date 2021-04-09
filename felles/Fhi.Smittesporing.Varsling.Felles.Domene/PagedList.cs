using System;
using System.Collections.Generic;
using System.Linq;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Optional;

namespace Fhi.Smittesporing.Varsling.Felles.Domene
{
    public interface IPagedQuery
    {
        Option<int> Sideantall { get; }
        Option<int> Sideindeks { get; }
    }

    public class PagedList<T>
    {
        public int TotaltAntall { get; set; }
        public int Sideindeks { get; set; }
        public int Sideantall { get; set; }

        public List<T> Resultater { get; set; }
        public int AntallSider => TotaltAntall / Sideantall + (TotaltAntall % Sideantall > 0 ? 1 : 0);

        public PagedList<TMapped> Map<TMapped>(Func<T, TMapped> mapFn)
        {
            return new PagedList<TMapped>
            {
                TotaltAntall = TotaltAntall,
                Sideantall = Sideantall,
                Sideindeks = Sideindeks,
                Resultater = Resultater.Select(mapFn).ToList()
            };
        }

        public PagedListAm<T> TilAm()
        {
            return new PagedListAm<T>
            {
                TotaltAntall = TotaltAntall,
                Sideantall = Sideantall,
                Sideindeks = Sideindeks,
                AntallSider = AntallSider,
                Resultater = Resultater
            };
        }

        public PagedListAm<TMapped> TilAm<TMapped>(Func<T, TMapped> mapFn)
        {
            return Map(mapFn).TilAm();
        }
    }
}