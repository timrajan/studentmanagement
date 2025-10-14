namespace StudentManagement.Models
{
    // This makes it easier to work with roles and prevents typos
    public static class StudentRole
    {
        public const string Creator = "Creator";
        public const string Viewer = "Viewer";
        
        // Helper method to check if a role is valid
        public static bool IsValid(string role)
        {
            return role == Creator || role == Viewer;
        }
        
        // Get all available roles (useful for dropdowns)
        public static List<string> GetAllRoles()
        {
            return new List<string> { Creator, Viewer };
        }
    }
}