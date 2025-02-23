namespace MyDemoApp.Models
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Employee
    {
        public Employee()
        {
            this.Departments = new HashSet<Department>();
            this.InverseManager = new HashSet<Employee>();
            this.EmployeesProjects = new HashSet<EmployeeProject>();
        }

        [Key]
        public int EmployeeID { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string JobTitle { get; set; } = null!;

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentID { get; set; }

        public virtual Department Department { get; set; } = null!;

        [ForeignKey(nameof(Manager))]
        public int? ManagerID { get; set; }

        public virtual Employee? Manager { get; set; }

        [ForeignKey(nameof(Address))]
        public int? AddressID { get; set; }

        public virtual Address? Address { get; set; }

        public virtual ICollection<Department> Departments { get; set; }

        public virtual ICollection<Employee> InverseManager { get; set; }
        
        public virtual ICollection<EmployeeProject> EmployeesProjects { get; set; }
    }
}
