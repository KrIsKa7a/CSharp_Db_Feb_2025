namespace AcademicRecordsApp.Data.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public int ExamId { get; set; }

        public virtual Exam Exam { get; set; } = null!;

        public int StudentId { get; set; }

        public virtual Student Student { get; set; } = null!;
    }
}
