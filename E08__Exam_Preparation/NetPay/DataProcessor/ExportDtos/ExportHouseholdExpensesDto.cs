namespace NetPay.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    using Data.Models;

    [XmlType(nameof(Household))]
    public class ExportHouseholdExpensesDto
    {
        [XmlElement(nameof(ContactPerson))]
        public string ContactPerson { get; set; } = null!;

        [XmlElement(nameof(Email))]
        public string? Email { get; set; }

        [XmlElement(nameof(PhoneNumber))]
        public string PhoneNumber { get; set; } = null!;

        [XmlArray(nameof(Expenses))]
        public ExportExpenseDto[] Expenses { get; set; } = null!;
    }
}
