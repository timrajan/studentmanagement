using StudentManagement.Models;

namespace StudentManagement.Services
{
    public class DataService
    {
        // Our "database" of users
        public List<User> Users { get; set; } = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "admin",
                FullName = "Super Administrator",
                Role = "SuperAdmin"
            },
            new User
            {
                Id = 2,
                Username = "user",
                FullName = "Team Admin User",
                Role = "TeamAdmin"
            },
            new User
            {
                Id = 3,
                Username = "student1",
                FullName = "Alice John",
                Role = "Student"
            }
        };

        // Our "database" of teams
        public List<Team> Teams { get; set; } = new List<Team>
        {
            new Team
            {
                Id = 1,
                Name = "Alpha Team",
                Description = "Frontend Development Team",
                CreatedDate = DateTime.Now.AddMonths(-6),
                TeamAdminName = "Sarah Johnson"
            },
            new Team
            {
                Id = 2,
                Name = "Beta Team",
                Description = "Backend Development Team",
                CreatedDate = DateTime.Now.AddMonths(-4),
                TeamAdminName = "Michael Chen"
            }
        };

        // Our "database" of team admins
        public List<TeamAdmin> TeamAdmins { get; set; } = new List<TeamAdmin>
        {
            new TeamAdmin
            {
                Id = 1,
                TeamId = 1,
                Name = "Sarah Johnson",
                Email = "sarah@school.com",
                Username = "User", // Windows username "User" is linked to Team Alpha
                AddedDate = DateTime.Now.AddMonths(-6)
            },
            new TeamAdmin
            {
                Id = 2,
                TeamId = 1,
                Name = "David Lee",
                Email = "david@school.com",
                Username = "", // No Windows username linked
                AddedDate = DateTime.Now.AddMonths(-5)
            },
            new TeamAdmin
            {
                Id = 3,
                TeamId = 2,
                Name = "Michael Chen",
                Email = "michael@school.com",
                Username = "", // No Windows username linked
                AddedDate = DateTime.Now.AddMonths(-4)
            }
        };

        // Our "database" of students
        public List<Student> Students { get; set; } = new List<Student>
        {
            new Student
            {
                Id = 1,
                Name = "Alice John",
                Email = "alice@alice.com",
                TeamId = 1,
                EnrollmentDate = DateTime.Now.AddMonths(-3),
                Role = "Creator"
            },
            new Student
            {
                Id = 2,
                Name = "Tim Alex",
                Email = "tim@tim.com",
                TeamId = 2,
                EnrollmentDate = DateTime.Now.AddMonths(-3),
                Role = "Viewer"
            },
            new Student
            {
                Id = 3,
                Name = "Rah Man",
                Email = "rah@rah.com",
                TeamId = 2,
                EnrollmentDate = DateTime.Now.AddMonths(-3),
                Role = "Creator"
            }
        };

        // Our "database" of study records
        public List<StudyRecord> StudyRecords { get; set; } = new List<StudyRecord>
        {
            new StudyRecord
            {
                Id = 1,
                StudentId = 1,
                Subject = "Mathematics",
                Topic = "Linear Algebra",
                HoursSpent = 2.5,
                Notes = "Worked on matrix multiplication problems. Need more practice.",
                StudyDate = DateTime.Now.AddDays(-2),
                CreatedDate = DateTime.Now.AddDays(-2)
            },
            new StudyRecord
            {
                Id = 2,
                StudentId = 1,
                Subject = "Computer Science",
                Topic = "ASP.NET Core MVC",
                HoursSpent = 3.0,
                Notes = "Built my first MVC application! Understanding Models, Views, and Controllers.",
                StudyDate = DateTime.Now.AddDays(-1),
                CreatedDate = DateTime.Now.AddDays(-1)
            },
            new StudyRecord
            {
                Id = 3,
                StudentId = 2,
                Subject = "Physics",
                Topic = "Quantum Mechanics",
                HoursSpent = 1.5,
                Notes = "Introduction to wave-particle duality. Fascinating stuff!",
                StudyDate = DateTime.Now.AddDays(-1),
                CreatedDate = DateTime.Now.AddDays(-1)
            },
            new StudyRecord
            {
                Id = 4,
                StudentId = 3,
                Subject = "Computer Science",
                Topic = "Data Structures",
                HoursSpent = 2.0,
                Notes = "Studied binary trees and traversal algorithms.",
                StudyDate = DateTime.Now,
                CreatedDate = DateTime.Now
            }
        };

        // Our "database" of sports records
        public List<SportsRecord> SportsRecords { get; set; } = new List<SportsRecord>
        {
            new SportsRecord
            {
                Id = 1,
                StudentId = 1,
                SportName = "Basketball",
                ActivityType = "Practice",
                HoursSpent = 2.0,
                Notes = "Worked on free throws and defense drills.",
                ActivityDate = DateTime.Now.AddDays(-3),
                CreatedDate = DateTime.Now.AddDays(-3)
            },
            new SportsRecord
            {
                Id = 2,
                StudentId = 3,
                SportName = "Soccer",
                ActivityType = "Match",
                HoursSpent = 1.5,
                Notes = "Played a friendly match. Scored 2 goals!",
                ActivityDate = DateTime.Now.AddDays(-1),
                CreatedDate = DateTime.Now.AddDays(-1)
            }
        };
    }
}
