using AutoMapper;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Datalag.Repositories;
using Fhi.Smittesporing.Varsling.Domene;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using Fhi.Smittesporing.Varsling.Domene.Utils;

namespace Fhi.Smittesporing.Varsling.Test.Domene
{
    public class DomeneTestBase
    {
        protected IIndekspasientRepository IndekspasientRepository;
        protected IMapper Mapper;
        protected CancellationToken CancellationToken = new CancellationToken();
        protected SmitteVarslingContext DbContext;
        protected Random Rand = new Random(DateTime.Now.Millisecond);

        public DomeneTestBase()
        {
            DbContext = new SmitteVarslingContext(new DbContextOptionsBuilder<SmitteVarslingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, new Arbeidskontekst());

            IndekspasientRepository = new IndekspasientRepository(DbContext);

            Mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomeneMapperprofil>();
            }));
        }

        protected string RandomTelefonNummer()
        {
            return Rand.Next(10000000, 99999999).ToString();
        }
    }
}
