using System;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Simula.InternApi.Requests
{
    public class HentKontaktrapportQuery : IRequest<Option<SimulaKontaktrapport>>
    {
        public Guid Id { get; set; }
    }
}