namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Part")]
    public class ImportPartDto
    {
        [Required]
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("price")]
        public string Price { get; set; } = null!;

        [Required]
        [XmlElement("quantity")]
        public string Quantity { get; set; } = null!;

        [Required]
        [XmlElement("supplierId")]
        public string SupplierId { get; set; } = null!;
    }
}
