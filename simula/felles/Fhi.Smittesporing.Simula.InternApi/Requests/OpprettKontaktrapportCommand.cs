using System;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Simula.InternApi.Requests
{
    public class OpprettKontaktrapportCommand : IRequest<Option<Guid>>
    {
        public SimulaKontaktrapport.OpprettCommand Data { get; set; }
    }
}
