namespace CarDealer.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("part")]
    public class ExportCarsWithPartsPartDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = null!;

        [XmlAttribute("price")]
        public string Price { get; set; } = null!;
    }
}
