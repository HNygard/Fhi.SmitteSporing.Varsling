using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Domene;
using Microsoft.EntityFrameworkCore;

namespace Fhi.Smittesporing.Varsling.Datalag
{
    public static class QueryableExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, IPagedQuery q)
        {
            var sideindeks = q.Sideindeks.ValueOr(0);
            var sideantall = q.Sideantall.ValueOr(100);
            var resultater = await queryable
                .Skip(sideantall * sideindeks)
                .Take(sideantall)
                .ToListAsync();
            var totaltAntall = await queryable.CountAsync();
            return new PagedList<T>
            {
                Resultater = resultater,
                TotaltAntall = totaltAntall,
                Sideindeks = sideindeks,
                Sideantall = sideantall
            };
        }
    }
}
