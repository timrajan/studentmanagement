using Microsoft.AspNetCore.Mvc;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class StudentController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? teamId)
        {
            // Check if user is SuperAdmin
            var role = ViewBag.Role as string;
            if (role != "SuperAdmin")
            {
                TempData["ErrorMessage"] = "Access denied. Only SuperAdmins can access User Management.";
                return RedirectToAction("Index", "Home");
            }

            // Get all teams for dropdown
            var teams = _context.Teams.ToList();
            ViewBag.Teams = teams;

            // Get students filtered by team if teamId is provided
            List<Student> students;
            if (teamId.HasValue && teamId.Value > 0)
            {
                students = _context.Students.Where(s => s.TeamId == teamId.Value).ToList();
                ViewBag.SelectedTeamId = teamId.Value;
            }
            else
            {
                // Show all students if no team selected
                students = _context.Students.ToList();
                ViewBag.SelectedTeamId = 0;
            }

            return View(students);
        }

        // NEW: Show students grouped by team
        public IActionResult ByTeam()
        {
            // We'll pass BOTH students and teams to the view
            // For now, just pass students
            var students = _context.Students.ToList();
            return View(students);
        }

        // GET: Show form to create a new student
        [HttpGet]
        public IActionResult Create()
        {
            // Pass list of teams so Team Admin can select which team
            // (Later, we'll restrict this to only THEIR team)
            var teams = _context.Teams.ToList();
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
                ViewBag.Teams = _context.Teams.ToList();
                return View(student);
            }

            // Set the enrollment date
            student.EnrollmentDate = DateTime.UtcNow;

            // Add to database
            _context.Students.Add(student);
            _context.SaveChanges();

            // Get the team name for the success message
            var team = _context.Teams.FirstOrDefault(t => t.Id == student.TeamId);

            // Show success message
            TempData["SuccessMessage"] = $"Student '{student.Name}' has been added to {team?.Name}!";

            // Redirect to the students list
            return RedirectToAction("Index");
        }

        // POST: Delete a student
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToAction("Index");
            }

            // Remove sports records for this student
            var sportsRecords = _context.SportsRecords.Where(sr => sr.StudentId == id).ToList();
            _context.SportsRecords.RemoveRange(sportsRecords);

            // Remove the student
            _context.Students.Remove(student);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"User '{student.Name}' has been deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
