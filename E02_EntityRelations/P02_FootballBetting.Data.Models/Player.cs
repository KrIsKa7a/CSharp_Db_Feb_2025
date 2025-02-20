namespace P02_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidationConstants.Player;

    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(PlayerNameMaxLength)]
        public string Name { get; set; } = null!;

        public int SquadNumber { get; set; } // This may be short or even byte but Judge :/
        
        public bool IsInjured { get; set; }

        [ForeignKey(nameof(Position))]
        public int PositionId { get; set; }

        public virtual Position Position { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }

        public virtual Team? Team { get; set; }

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }

        public virtual Town Town { get; set; } = null!;

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }
            = new HashSet<PlayerStatistic>();
    }
}
