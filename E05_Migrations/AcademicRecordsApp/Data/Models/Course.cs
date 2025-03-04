namespace AcademicRecordsApp.Data.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public virtual ICollection<Exam> Exams { get; set; }
            = new HashSet<Exam>();

        public virtual ICollection<StudentCourse> Students { get; set; }
            = new HashSet<StudentCourse>();
    }
}
