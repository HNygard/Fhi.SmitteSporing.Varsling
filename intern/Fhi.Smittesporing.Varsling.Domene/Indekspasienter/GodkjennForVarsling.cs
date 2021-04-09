using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class GodkjennForVarsling
    {
        public class Command : IRequest
        {
            public int[] IndekspasientIder { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;

            public Handler(IIndekspasientRepository indekspasientRepository)
            {
                _indekspasientRepository = indekspasientRepository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var indekspasienter = await _indekspasientRepository.HentForIder(request.IndekspasientIder);

                if (indekspasienter.Any(x => !x.KanGodkjennesForVarsling))
                {
                    throw new ArgumentException($"En eller flere angitte indekspasienter kan ikke godkjennes for varsling.");
                }

                foreach (var indekspasient in indekspasienter)
                {
                    await _indekspasientRepository.OppdaterVarslingsstatus(indekspasient.IndekspasientId,
                        Varslingsstatus.Godkjent);
                }

                return Unit.Value;
            }
        }
    }
}