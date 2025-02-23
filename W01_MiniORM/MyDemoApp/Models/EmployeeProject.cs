namespace MyDemoApp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmployeeProject
    {
        [Key]
        [ForeignKey(nameof(Employee))]
        public int EmployeeID { get; set; }

        public virtual Employee Employee { get; set; } = null!;

        [Key]
        [ForeignKey(nameof(Project))]
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; } = null!;
    }
}
