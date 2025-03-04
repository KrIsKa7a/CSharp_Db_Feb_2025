namespace AcademicRecordsApp.Data.Models
{
    public class Exam
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;

        public virtual ICollection<Grade> Grades { get; set; }
            = new HashSet<Grade>();
    }
}
