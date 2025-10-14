namespace StudentManagement.Models
{
    public class TeamAdmin
    {
        // Unique ID for each Team Admin
        public int Id { get; set; }
        
        // Which team does this admin belong to?
        public int TeamId { get; set; }
        
        // Admin's full name
        public string Name { get; set; } = string.Empty;
        
        // Admin's email
        public string Email { get; set; } = string.Empty;
        
        // When was this admin added?
        public DateTime AddedDate { get; set; }
    }
}