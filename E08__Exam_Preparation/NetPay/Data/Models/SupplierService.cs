namespace NetPay.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SupplierService
    {
        [Required]
        [ForeignKey(nameof(Supplier))]
        public int SupplierId { get; set; } // Required can be omitted, this is part of the PK

        public virtual Supplier Supplier { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }

        public virtual Service Service { get; set; } = null!;
    }
}
