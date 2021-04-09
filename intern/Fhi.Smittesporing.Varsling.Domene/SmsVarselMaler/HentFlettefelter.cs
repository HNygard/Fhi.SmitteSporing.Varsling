using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.SmsVarselMaler
{
    public class HentFlettefelter
    {
        public class Query : IRequest<List<SmsFlettefeltAm>>
        {

        }

        public class Handler : IRequestHandler<Query, List<SmsFlettefeltAm>>
        {
            private readonly IMapper _mapper;

            public Handler(IMapper mapper)
            {
                _mapper = mapper;
            }

            public Task<List<SmsFlettefeltAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var flettefelter = SmsVarselMal.TilgjengeligeFlettefelter;

                return Task.FromResult(flettefelter.Select(_mapper.Map<SmsFlettefeltAm>).ToList());
            }
        }
    }
}