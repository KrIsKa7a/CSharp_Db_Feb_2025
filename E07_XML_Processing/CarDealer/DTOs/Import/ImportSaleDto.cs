namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Sale")]
    public class ImportSaleDto
    {
        [Required]
        [XmlElement("carId")]
        public string CarId { get; set; } = null!;

        [Required]
        [XmlElement("customerId")]
        public string CustomerId { get; set; } = null!;

        [Required]
        [XmlElement("discount")]
        public string Discount { get; set; } = null!;
    }
}
