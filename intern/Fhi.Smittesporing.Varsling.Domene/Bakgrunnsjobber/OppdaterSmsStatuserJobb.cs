using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.SmsVarsler;
using MediatR;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class OppdaterSmsStatuserJobb : IPeriodiskJobb
    {
        private readonly Konfig _konfig;
        private readonly IMediator _mediator;

        public OppdaterSmsStatuserJobb(IOptions<Konfig> konfig, IMediator mediator)
        {
            _mediator = mediator;
            _konfig = konfig.Value;
        }

        public async Task<bool> UtforJobb(CancellationToken stoppingToken)
        {
            var antallOppdateringer = await _mediator.Send(new OppdaterSmsStatuser.Command
            {
                Batchstorrelse = _konfig.Batchstorrelse
            }, stoppingToken);

            return antallOppdateringer > 0;
        }

        public class Konfig : IPeriodiskJobbKonfig
        {
            public JobbIntervallKonfig JobbIntervaller { get; set; }
            public int Batchstorrelse { get; set; } = 100;
        }
    }
}