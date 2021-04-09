using System;
using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Interop.Api.Models
{
    /// <summary>
    /// Model of metadata collected alongside contact keys
    /// </summary>
    public class ExposedKeyMetadataDto
    {
        /// <summary>
        /// Reception time of the message (ISO8601)
        /// </summary>
        [Required]
        public DateTime ReceptionTime { get; set; }

        /// <summary>
        /// Signal strength RSSI (in dBm)
        /// </summary>
        /// <example>-42</example>
        [Required]
        public int SignalStrengthRssi { get; set; }

        /// <summary>
        /// Device calibration flag
        /// </summary>
        /// <example>1</example>
        [Required]
        public int CalibrationFlag { get; set; }

        /// <summary>
        /// Optional: Transmission power
        /// </summary>
        /// <example>null</example>
        public int? TxPower { get; set; }
    }
}