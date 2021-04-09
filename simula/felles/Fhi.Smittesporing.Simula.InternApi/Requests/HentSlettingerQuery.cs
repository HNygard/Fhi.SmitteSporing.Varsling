using System.Collections.Generic;
using MediatR;

namespace Fhi.Smittesporing.Simula.InternApi.Requests
{
    public class HentSlettingerQuery : IRequest<List<string>>
    {
        public IEnumerable<string> Telefonnummer { get; set; }
    }
}