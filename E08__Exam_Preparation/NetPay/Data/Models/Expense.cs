namespace NetPay.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Enums;
    using static Common.EntityValidationConstants.Expense;

    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ExpenseNameMaxLength)]
        public string ExpenseName { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; } // Required by default

        [Required]
        public DateTime DueDate { get; set; } // Required by default

        [Required]
        public PaymentStatus PaymentStatus { get; set; } // Required by default -> enums are stored in DB as integer -> enum is named value of integer

        [Required]
        [ForeignKey(nameof(Household))]
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }

        public virtual Service Service { get; set; } = null!;
    }
}
