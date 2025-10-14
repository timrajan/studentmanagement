namespace StudentManagement.Models
{
    public class StudyRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public double HoursSpent { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime StudyDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}