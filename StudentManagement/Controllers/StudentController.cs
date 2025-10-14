using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;

namespace StudentManagement.Contollers
{
    public class StudentController : Controller
    {

        private static List<Team> teams = new List<Team>
        {
            new Team
            {
                Id = 1,
                Name = "Alpha Team",
                Description = "Focus on frontend development",
                CreatedDate = DateTime.Now.AddMonths(-6),
                TeamAdminName = "Sarah Johnson"
            },
            new Team
            {
                Id = 2,
                Name = "Beta Team",
                Description = "Focus on backend systems",
                CreatedDate = DateTime.Now.AddMonths(-4),
                TeamAdminName = "Michael Chen"
            }
        };
        

        private static List<Student> students = new List<Student>
        {
            new Student
            {
                Id = 1,
                Name = "Alice John",
                Email = "alice@alice.com",
                TeamId = 1,
                EnrollmentDate = DateTime.Now.AddMonths(-3)
            },
            new Student
            {
                Id = 2,
                Name = "Tim Alex",
                Email = "tim@tim.com",
                TeamId = 2,
                EnrollmentDate = DateTime.Now.AddMonths(-3)
            },
            new Student
            {
                Id = 3,
                Name = "Rah Man",
                Email = "rah@rah.com",
                TeamId = 2,
                EnrollmentDate = DateTime.Now.AddMonths(-3)
            }
        };

        public IActionResult Index()
        {
            return View(students);
        }

        // NEW: Show students grouped by team
        public IActionResult ByTeam()
        {
            // We'll pass BOTH students and teams to the view
            // For now, just pass students
            return View(students);
        }
        
       

        // GET: Show form to create a new student
        [HttpGet]
        public IActionResult Create()
        {
            // Pass list of teams so Team Admin can select which team
            // (Later, we'll restrict this to only THEIR team)
            ViewBag.Teams = teams;
            return View();
        }

        // POST: Receive form data and create the student
        [HttpPost]
        public IActionResult Create(Student student)
        {
            // Validate the data
            if (string.IsNullOrEmpty(student.Name) || 
                string.IsNullOrEmpty(student.Email) ||
                student.TeamId == 0)
            {
                ViewBag.Error = "Name, Email, and Team are required fields.";
                ViewBag.Teams = teams;
                return View(student);
            }
            
            // Generate a new ID
            student.Id = students.Count > 0 
                ? students.Max(s => s.Id) + 1 
                : 1;
            
            // Set the enrollment date
            student.EnrollmentDate = DateTime.Now;
            
            // Add to our list
            students.Add(student);
            
            // Get the team name for the success message
            var team = teams.FirstOrDefault(t => t.Id == student.TeamId);
            
            // Show success message
            TempData["SuccessMessage"] = $"Student '{student.Name}' has been added to {team?.Name}!";
            
            // Redirect to the students list
            return RedirectToAction("Index");
}
    }
}