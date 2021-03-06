using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Interop.Api.Models
{
    /// <summary>
    /// Batch of exposed keys
    /// </summary>
    public class ExposedKeysBatchDto
    {
        /// <summary>
        /// The ID of the batch (generated by the sender)
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// The exposed keys in this batch
        /// </summary>
        [Required]
        public IEnumerable<ExposedKeyDto> ExposedKeys { get; set; }
    }
}