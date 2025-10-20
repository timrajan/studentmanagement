using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class SportsRecordController : BaseController
    {
        private readonly DataService _dataService;

        public SportsRecordController(DataService dataService)
        {
            _dataService = dataService;
        }

        // Show the main sports record page with buttons
        public IActionResult Index()
        {
            return View();
        }

        // Show all sports records
        public IActionResult AllRecords()
        {
            return View(_dataService.SportsRecords);
        }

        // GET: Show form to select a student
        [HttpGet]
        public IActionResult SelectStudent()
        {
            ViewBag.Students = _dataService.Students;
            return View();
        }

        // GET: Show form to select a sport type
        [HttpGet]
        public IActionResult SelectSport()
        {
            // Get unique sport names from sports records
            var sports = _dataService.SportsRecords
                .Select(r => r.SportName)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Sports = sports;
            return View();
        }

        // Show sports records by sport name
        public IActionResult BySport(string sport)
        {
            var records = _dataService.SportsRecords
                .Where(r => r.SportName.Equals(sport, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewBag.Sport = sport;
            return View(records);
        }

        // Show sports records for a specific student
        public IActionResult ByStudent(int studentId)
        {
            var studentRecords = _dataService.SportsRecords
                .Where(r => r.StudentId == studentId)
                .ToList();

            var student = _dataService.Students.FirstOrDefault(s => s.Id == studentId);

            ViewBag.StudentName = student?.Name ?? "Unknown Student";
            ViewBag.StudentId = studentId;

            return View(studentRecords);
        }

        // GET: Show the form to create a new sports record
        [HttpGet]
        public IActionResult Create(int? studentId)
        {
            // If studentId is provided, check if they have Creator role
            if (studentId.HasValue)
            {
                var student = _dataService.Students.FirstOrDefault(s => s.Id == studentId.Value);

                if (student == null)
                {
                    TempData["ErrorMessage"] = "Student not found.";
                    return RedirectToAction("Index");
                }

                if (student.Role != "Creator")
                {
                    TempData["ErrorMessage"] = $"{student.Name} does not have permission to create sports records. Only students with 'Creator' role can add records.";
                    return RedirectToAction("Index");
                }

                // Pre-select this student in the form
                ViewBag.SelectedStudentId = studentId.Value;
            }

            // Filter students list to only show Creators
            ViewBag.Students = _dataService.Students.Where(s => s.Role == "Creator").ToList();

            return View();
        }

        // POST: Receive the form data and save it
        [HttpPost]
        public IActionResult Create(SportsRecord record)
        {
            // Validate the data
            if (string.IsNullOrEmpty(record.SportName) ||
                string.IsNullOrEmpty(record.ActivityType) ||
                record.HoursSpent <= 0)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                ViewBag.Students = _dataService.Students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }

            // IMPORTANT: Check if the student has Creator role
            var student = _dataService.Students.FirstOrDefault(s => s.Id == record.StudentId);

            if (student == null)
            {
                ViewBag.Error = "Selected student not found.";
                ViewBag.Students = _dataService.Students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }

            if (student.Role != "Creator")
            {
                ViewBag.Error = $"{student.Name} does not have permission to create sports records. Only 'Creator' role can add records.";
                ViewBag.Students = _dataService.Students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }

            // Generate a new ID
            record.Id = _dataService.SportsRecords.Count > 0
                ? _dataService.SportsRecords.Max(r => r.Id) + 1
                : 1;

            // Set the created date
            record.CreatedDate = DateTime.Now;

            // If ActivityDate wasn't set, use today
            if (record.ActivityDate == DateTime.MinValue)
            {
                record.ActivityDate = DateTime.Now;
            }

            // Add to our list
            _dataService.SportsRecords.Add(record);

            // Redirect to the index page
            TempData["SuccessMessage"] = $"Sports record created successfully by {student.Name}!";
            return RedirectToAction("Index");
        }
    }
}
