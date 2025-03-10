namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Car")]
    public class ImportCarDto
    {
        [Required]
        [XmlElement("make")]
        public string Make { get; set; } = null!;

        [Required]
        [XmlElement("model")]
        public string Model { get; set; } = null!;

        [Required]
        [XmlElement("traveledDistance")]
        public string TraveledDistance { get; set; } = null!;

        [XmlArray("parts")]
        public ImportCarPartDto[]? Parts { get; set; }
    }
}
