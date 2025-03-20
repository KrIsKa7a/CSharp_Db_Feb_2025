namespace NetPay.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using static Common.EntityValidationConstants;
    using static Common.EntityValidationConstants.Household;

    [XmlType(nameof(Household))]
    public class ImportHouseholdDto
    {
        [Required]
        [MinLength(ContactPersonMinLength)]
        [MaxLength(ContactPersonMaxLength)]
        [XmlElement(nameof(ContactPerson))]
        public string ContactPerson { get; set; } = null!;

        [MinLength(EmailMinLength)]
        [MaxLength(EmailMaxLength)]
        [XmlElement(nameof(Email))]
        public string? Email { get; set; }

        [Required]
        [MinLength(PhoneNumberLength)]
        [MaxLength(PhoneNumberLength)]
        [RegularExpression(PhoneNumberValidationRegex)]
        [XmlAttribute("phone")]
        public string PhoneNumber { get; set; } = null!;
    }
}
