using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.SmsTestmeldinger
{
    public class HentHendelser
    {
        public class Query : IRequest<List<SmsStatusoppdateringAm>>
        {
            public Guid Referanse { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<SmsStatusoppdateringAm>>
        {
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly IMapper _mapper;

            public Handler(ISmsTjenesteFacade smsTjenesteFacade, IMapper mapper)
            {
                _smsTjenesteFacade = smsTjenesteFacade;
                _mapper = mapper;
            }

            public async Task<List<SmsStatusoppdateringAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var oppdateringer = await _smsTjenesteFacade.HentStatusoppdateringerForSms(request.Referanse);

                return oppdateringer.Select(_mapper.Map<SmsStatusoppdateringAm>).ToList();
            }
        }
    }
}