namespace MyDemoApp.Models
{
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    public class Town
    {
        public Town()
        {
            this.Addresses = new HashSet<Address>();
        }

        [Key]
        public int TownID { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Address> Addresses { get; set; }
    }
}
