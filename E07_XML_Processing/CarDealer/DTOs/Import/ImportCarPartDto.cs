namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [Required]
        [XmlAttribute("id")]
        public string Id { get; set; } = null!;
    }
}
