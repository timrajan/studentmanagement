namespace StudentManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int TeamId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        // Student's phone number (optional)
        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = "Viewer";  
    }
}