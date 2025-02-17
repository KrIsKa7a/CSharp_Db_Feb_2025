﻿namespace SoftUni.Models
{
    using System;
    using System.Collections.Generic;

    public class Employee
    {
        public Employee()
        {
            this.Departments = new HashSet<Department>();
            this.InverseManager = new HashSet<Employee>();
            //this.Projects = new HashSet<Project>();
            this.EmployeesProjects = new HashSet<EmployeeProject>();
        }

        public int EmployeeId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string JobTitle { get; set; } = null!;

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; } = null!;

        public int? ManagerId { get; set; }

        public virtual Employee? Manager { get; set; }

        public int? AddressId { get; set; }

        public virtual Address? Address { get; set; }

        public virtual ICollection<Department> Departments { get; set; }

        public virtual ICollection<Employee> InverseManager { get; set; }

        //public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<EmployeeProject> EmployeesProjects { get; set; }
    }
}
