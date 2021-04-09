using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    /// <summary>
    /// Fjerner alle utgåtte data for gitt UtgattTidspunkt
    /// TODO: generaliser navngiving
    /// </summary>
    public class FjernUtgatteIndekspasienter
    {
        public class Command : IRequest<int>
        {
            public DateTime UtgattTidspunkt { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;

            public Handler(IIndekspasientRepository indekspasientRepository)
            {
                _indekspasientRepository = indekspasientRepository;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                
                return await _indekspasientRepository.FjernUtgattData(request.UtgattTidspunkt);
            }
        }
    }
}