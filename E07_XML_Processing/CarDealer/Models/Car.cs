namespace CarDealer.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Make { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        public long TraveledDistance { get; set; }

        public virtual ICollection<Sale> Sales { get; set; } 
            = new HashSet<Sale>();    

        public virtual ICollection<PartCar> PartsCars { get; set; } 
            = new HashSet<PartCar>();
    }
}
