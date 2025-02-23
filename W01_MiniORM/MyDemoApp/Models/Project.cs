namespace MyDemoApp.Models
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    public class Project
    {
        public Project()
        {
            this.EmployeesProjects = new HashSet<EmployeeProject>();
        }

        [Key]
        public int ProjectID { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual ICollection<EmployeeProject> EmployeesProjects { get; set; }
    }
}
