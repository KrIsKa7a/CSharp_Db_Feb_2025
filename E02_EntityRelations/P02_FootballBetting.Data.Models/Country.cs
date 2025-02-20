namespace P02_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Country;

    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(CountryNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Town> Towns { get; set; }
            = new HashSet<Town>();
    }
}
