namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Supplier")]
    public class ImportSupplierDto
    {
        // Two types of attributes used in DTO:
        // 1. Validation attributes -> define the constraints for validation of deserialized data (only when deserializing)
        // 2. XmlSerializer attributes -> define constraint for XmlSerializer for how to serialize/deserialize the objects into Xml
        [Required]
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("isImporter")]
        public string IsImporter { get; set; } = null!;
    }
}
