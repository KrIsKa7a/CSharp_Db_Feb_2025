namespace MyDemoApp.Models
{
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Address
    {
        public Address()
        {
            this.Employees = new HashSet<Employee>();
        }

        [Key]
        public int AddressID { get; set; }

        public string AddressText { get; set; } = null!;

        [ForeignKey(nameof(Town))]
        public int? TownID { get; set; }

        public virtual Town? Town { get; set; }
        
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
