﻿namespace SoftUni.Models
{
    using System;
    using System.Collections.Generic;

    public class Project
    {
        public Project()
        {
            //this.Employees = new HashSet<Employee>();
            this.EmployeesProjects = new HashSet<EmployeeProject>();
        }

        public int ProjectId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<EmployeeProject> EmployeesProjects { get; set; }
    }
}
