namespace StudentManagement.Models
{
    public class StudyRecord
    {
        public int Id { get; set; }

        // Left column fields
        public string? Team { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? EmailAddress { get; set; }
        public string? StudentIdentityID { get; set; }
        public string? StudentInitialID { get; set; }

        // Right column fields
        public string? Environment { get; set; }
        public string? StudentIQLevel { get; set; }
        public string? StudentRollNumber { get; set; }
        public string? StudentRollName { get; set; }
        public string? StudentParentEmailAddress { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Tags { get; set; }
        public string? Comments { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}