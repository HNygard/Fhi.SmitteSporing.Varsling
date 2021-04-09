using System.Xml.Serialization;

namespace Fhi.Smittesporing.Helsenorge.Api.Models
{
    public partial class Table
    {
        /// <remarks/>
        [XmlElement]
        public RadInnhold[] Rader { get; set; }
    }

    public class RadInnhold
    {
        [XmlElement]
        public string[] Cell;
    }
}
