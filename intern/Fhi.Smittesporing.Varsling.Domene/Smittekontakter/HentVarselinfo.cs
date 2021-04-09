using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;
using Optional;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class HentVarselinfo
    {
        public class Query : IRequest<Option<List<SmsVarselAm>>>
        {
            public int SmittekontaktId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<List<SmsVarselAm>>>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly IMapper _mapper;
            private readonly ISmsTjenesteFacade _smsTjeneste;

            public Handler(ISmittekontaktRespository smittekontaktRespository, IMapper mapper, ISmsTjenesteFacade smsTjeneste)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _mapper = mapper;
                _smsTjeneste = smsTjeneste;
            }

            public async Task<Option<List<SmsVarselAm>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var smittekontakt = await _smittekontaktRespository.HentForIdMedVarsler(request.SmittekontaktId);

                return await smittekontakt.MapAsync(async s =>
                {
                    var varsler = new List<SmsVarselAm>();
                    foreach (var varsel in s.SmsVarsler)
                    {
                        var varselAm = _mapper.Map<SmsVarselAm>(varsel);
                        varselAm.Oppdateringer = (await varsel.Referanse
                            .ToOption()
                            .MapAsync(r => _smsTjeneste.HentStatusoppdateringerForSms(r)))
                            .Map(oppdateringer => oppdateringer.Select(o => _mapper.Map<SmsStatusoppdateringAm>(o)).ToList())
                            .ValueOr(new List<SmsStatusoppdateringAm>());
                        varsler.Add(varselAm);
                    }
                    return varsler;
                });
            }
        }
    }
}