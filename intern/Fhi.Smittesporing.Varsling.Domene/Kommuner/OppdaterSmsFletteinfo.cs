using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Kommuner
{
    public class OppdaterSmsFletteinfo
    {
        public class Command : IRequest
        {
            public int KommuneId { get; set; }
            public string SmsFletteinfo { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IKommuneRepository _kommuneRepository;

            public Handler(IKommuneRepository kommuneRepository)
            {
                _kommuneRepository = kommuneRepository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var kommune = (await _kommuneRepository.HentForId(request.KommuneId))
                    .ValueOr(() => throw new Exception("Finnes ingen kommune med ID=" + request.KommuneId));

                kommune.SmsFletteinfo = request.SmsFletteinfo;

                await _kommuneRepository.Lagre();

                return Unit.Value;
            }
        }
    }
}