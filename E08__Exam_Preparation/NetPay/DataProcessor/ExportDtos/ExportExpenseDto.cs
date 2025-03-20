namespace NetPay.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    using Data.Models;

    [XmlType(nameof(Expense))]
    public class ExportExpenseDto
    {
        [XmlElement(nameof(ExpenseName))]
        public string ExpenseName { get; set; } = null!;

        [XmlElement(nameof(Amount))]
        public string Amount { get; set; } = null!;

        [XmlElement(nameof(PaymentDate))]
        public string PaymentDate { get; set; } = null!;

        [XmlElement(nameof(ServiceName))]
        public string ServiceName { get; set; } = null!;
    }
}
