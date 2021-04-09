using System;
using Fhi.Smittesporing.Interop.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Interop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExposedKeysController : ControllerBase
    {
        /// <summary>
        /// Send a batch of exposed keys
        /// </summary>
        /// <param name="id">The ID of the batch being sent</param>
        /// <param name="data">The contents of the exposed keys batch</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] ExposedKeysBatchDto data)
        {
            return NoContent();
        }
    }
}
