using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class StudentController : BaseController
    {
        private readonly DataService _dataService;

        public StudentController(DataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            return View(_dataService.Students);
        }

        // NEW: Show students grouped by team
        public IActionResult ByTeam()
        {
            // We'll pass BOTH students and teams to the view
            // For now, just pass students
            return View(_dataService.Students);
        }
        
       

        // GET: Show form to create a new student
        [HttpGet]
        public IActionResult Create()
        {
            // Pass list of teams so Team Admin can select which team
            // (Later, we'll restrict this to only THEIR team)
            ViewBag.Teams = _dataService.Teams;
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
                ViewBag.Teams = _dataService.Teams;
                return View(student);
            }

            // Generate a new ID
            student.Id = _dataService.Students.Count > 0
                ? _dataService.Students.Max(s => s.Id) + 1
                : 1;

            // Set the enrollment date
            student.EnrollmentDate = DateTime.Now;

            // Add to our list
            _dataService.Students.Add(student);

            // Get the team name for the success message
            var team = _dataService.Teams.FirstOrDefault(t => t.Id == student.TeamId);

            // Show success message
            TempData["SuccessMessage"] = $"Student '{student.Name}' has been added to {team?.Name}!";

            // Redirect to the students list
            return RedirectToAction("Index");
}
    }
}