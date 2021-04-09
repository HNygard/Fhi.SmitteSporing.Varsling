using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class SettVarslingFerdig
    {
        public class Command : IRequest
        {
            public int IndekspasientId { get; set; }
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
                var indekspasient = (await _indekspasientRepository.HentForIder(new []{ request.IndekspasientId }))
                    .FirstOrDefault() ?? throw new ArgumentException("Finnes ikke indekspasient for angitt ID");

                if (!indekspasient.KanSettesFerdig)
                {
                    throw new ArgumentException("Angitt indekspasient kan ikke settes ferdig.");
                }

                await _indekspasientRepository.OppdaterVarslingsstatus(indekspasient.IndekspasientId,
                    Varslingsstatus.Ferdig);

                return Unit.Value;
            }
        }
    }
}