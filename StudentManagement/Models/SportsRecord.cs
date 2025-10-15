namespace StudentManagement.Models
{
    public class SportsRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public double HoursSpent { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
