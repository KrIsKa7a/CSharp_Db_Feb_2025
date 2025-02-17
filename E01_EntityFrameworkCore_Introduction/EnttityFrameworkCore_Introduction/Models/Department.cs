namespace SoftUni.Models
{
    using System.Collections.Generic;

    public class Department
    {
        public Department()
        {
            this.Employees = new HashSet<Employee>();
        }

        public int DepartmentId { get; set; }

        public string Name { get; set; } = null!;

        // The employee which is manager of this department
        public int ManagerId { get; set; }

        public virtual Employee Manager { get; set; } = null!;

        // Employees that work in this Department
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
