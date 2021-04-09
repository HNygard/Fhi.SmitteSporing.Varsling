using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional.Collections;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class HentVarslingssimulering
    {
        public class Query : IRequest<VarslingssimuleringAm>
        {
            public int IndekspasientId { get; set; }
        }

        public class Handler : IRequestHandler<Query, VarslingssimuleringAm>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly List<IVarslingsregel> _varslingsregler;

            public Handler(ISmittekontaktRespository smittekontaktRespository, IEnumerable<IVarslingsregel> varslingsregler)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _varslingsregler = varslingsregler.ToList();
            }

            public async Task<VarslingssimuleringAm> Handle(Query request, CancellationToken cancellationToken)
            {
                var smittekontakterForIndekspasient = await _smittekontaktRespository
                    .HentSmittekontaktTilVarslingForIndekspasient(request.IndekspasientId);

                var smittekontaktResultater = smittekontakterForIndekspasient.Select(x =>
                {
                    var regelVurderinger = _varslingsregler
                        .GroupBy(r => r.KanVarsles(x))
                        .ToDictionary(g => g.Key, g => g.Select(r => new VarslingsregelAm
                        {
                            Navn = r.Navn,
                            Beskrivelse = r.Beskrivelse
                        }));

                    return new VarslingssimuleringdetaljerAm
                    {
                        SmittekontaktId = x.SmittekontaktId,
                        KanVarsles = regelVurderinger.GetValueOrNone(false)
                            .Map(avvistRegler => !avvistRegler.Any())
                            .ValueOr(true),
                        VarselIkkeTillatAvRegler = regelVurderinger
                            .GetValueOrNone(false)
                            .ValueOr(Enumerable.Empty<VarslingsregelAm>()),
                        VarselTillatAvRegler = regelVurderinger
                            .GetValueOrNone(true)
                            .ValueOr(Enumerable.Empty<VarslingsregelAm>()),
                    };
                }).ToList();

                return new VarslingssimuleringAm
                {
                    IndekspasientId = request.IndekspasientId,
                    AntallKontakter = smittekontaktResultater.Count,
                    AntallKontakterTilVarsling = smittekontaktResultater.Count(d => d.KanVarsles),
                    AntallKontakterUtenVarsling = smittekontaktResultater.Count(d => !d.KanVarsles),
                    Detaljer = smittekontaktResultater
                };
            }
        }
    }
}