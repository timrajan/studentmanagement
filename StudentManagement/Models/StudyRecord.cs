namespace StudentManagement.Models
{
    public class StudyRecord
    {
        public int Id { get; set; }

        // Left column fields
        public string Team { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string StudentIdentityID { get; set; } = string.Empty;
        public string StudentInitialID { get; set; } = string.Empty;

        // Right column fields
        public string Environment { get; set; } = string.Empty;
        public string StudentIQLevel { get; set; } = string.Empty;
        public string StudentRollNumber { get; set; } = string.Empty;
        public string StudentRollName { get; set; } = string.Empty;
        public string StudentParentEmailAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}