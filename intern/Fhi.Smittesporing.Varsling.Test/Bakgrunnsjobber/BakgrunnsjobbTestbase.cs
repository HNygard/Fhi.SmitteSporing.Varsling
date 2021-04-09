using System;
using System.Threading;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Datalag.Repositories;
using Fhi.Smittesporing.Varsling.Domene;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Utils;
using Fhi.Smittesporing.Varsling.Test.DataLag;
using Microsoft.EntityFrameworkCore;

namespace Fhi.Smittesporing.Varsling.Test.Bakgrunnsjobber
{
    public class BakgrunnsjobbTestbase
    {
        protected IIndekspasientRepository IndekspasientRepository;
        protected ISmittekontaktRespository SmittekontaktRespository;
        protected ITelefonRespository TelefonRespository;
        protected IMapper Mapper;
        protected CancellationToken CancellationToken = new CancellationToken();
        protected SmitteVarslingContext DbContext;
        protected Random Rand = new Random(DateTime.Now.Millisecond);
        protected ICryptoManagerFacade CryptoManagerFacade;
        protected ITelefonNormalFacade TelefonNormalFacade;
  
        public BakgrunnsjobbTestbase()
        {
            DbContext = new SmitteVarslingContext(new DbContextOptionsBuilder<SmitteVarslingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, new Arbeidskontekst());
    
            IndekspasientRepository = new IndekspasientRepository(DbContext);
            SmittekontaktRespository = new SmittekontaktRespository(DbContext);
            TelefonRespository = new TelefonRepository(DbContext);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomeneMapperprofil>();
            });
            Mapper = new Mapper(mapperConfig);


            CryptoManagerFacade = new TestCryptoManagerFacade();
            TelefonNormalFacade = new TelefonNormalFacade();
        }
    }
}
