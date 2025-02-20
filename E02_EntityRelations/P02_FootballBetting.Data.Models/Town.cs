namespace P02_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidationConstants.Town;

    public class Town
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(TownNameMaxLength)]
        public string Name { get; set; } = null!;

        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; } = null!;

        public virtual ICollection<Team> Teams { get; set; }
            = new HashSet<Team>();

        public virtual ICollection<Player> Players { get; set; }
            = new HashSet<Player>();
    }
}
