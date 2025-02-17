namespace SoftUni.Models
{
    using System.Collections.Generic;

    public class Address
    {
        public Address()
        {
            this.Employees = new HashSet<Employee>();
        }

        public int AddressId { get; set; }

        public string AddressText { get; set; } = null!;

        // One side of relation navigation prop
        public int? TownId { get; set; }

        public virtual Town? Town { get; set; }

        // Many side of relation navigation prop
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
