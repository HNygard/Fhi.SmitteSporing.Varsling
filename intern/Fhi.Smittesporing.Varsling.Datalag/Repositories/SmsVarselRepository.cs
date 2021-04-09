using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class SmsVarselRepository : ISmsVarselRepository
    {
        private readonly SmitteVarslingContext _dbContext;

        public SmsVarselRepository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OpprettSmsVarsler(IEnumerable<SmsVarsel> smsVarsler)
        {
            foreach (var smsVarsel in smsVarsler)
            {
                _dbContext.SmsVarsler.Add(smsVarsel);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Option<SmsVarsel>> FinnForReferanse(Guid referanse)
        {
            return (await _dbContext.SmsVarsler.FirstOrDefaultAsync(x => x.Referanse == referanse)).SomeNotNull();
        }

        public async Task<int> Lagre()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}