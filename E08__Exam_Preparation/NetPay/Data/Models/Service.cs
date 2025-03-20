namespace NetPay.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Service;

    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ServiceNameMaxLength)]
        public string ServiceName { get; set; } = null!;

        public virtual ICollection<Expense> Expenses { get; set; }
            = new HashSet<Expense>();

        public virtual ICollection<SupplierService> SuppliersServices { get; set; }
            = new HashSet<SupplierService>();
    }
}
