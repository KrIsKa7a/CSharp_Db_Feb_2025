namespace CarDealer.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public bool IsImporter { get; set; }

        public virtual ICollection<Part> Parts { get; set; } 
            = new HashSet<Part>();
    }
}
