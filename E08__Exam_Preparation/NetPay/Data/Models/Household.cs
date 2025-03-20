namespace NetPay.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidationConstants.Household;

    public class Household
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ContactPersonMaxLength)]
        public string ContactPerson { get; set; } = null!;

        [MaxLength(EmailMaxLength)]
        public string? Email { get; set; }

        [Required]
        [Column(TypeName = PhoneNumberColumnType)]
        public string PhoneNumber { get; set; } = null!;

        public virtual ICollection<Expense> Expenses { get; set; }
            = new HashSet<Expense>();
    }
}
