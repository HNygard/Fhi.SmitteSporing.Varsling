using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Interop.Api.Models
{
    /// <summary>
    /// Model of exposed contact key with metadata for risk evaluation
    /// </summary>
    public class ExposedKeyDto
    {
        /// <summary>
        /// The raw contact key payload, base64 encoded
        /// </summary>
        /// <example>U2FtcGxlIGNvbnRhY3Qga2V5IHBheWxvYWQ=</example>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Metadata collected alongside the raw contact key payload
        /// </summary>
        [Required]
        public ExposedKeyMetadataDto Metadata { get; set; }
    }
}