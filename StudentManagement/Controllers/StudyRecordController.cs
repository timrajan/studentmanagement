using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class StudyRecordController : BaseController
    {
        private readonly DataService _dataService;

        public StudyRecordController(DataService dataService)
        {
            _dataService = dataService;
        }

        // Show the main study record page with buttons
        public IActionResult Index()
        {
            return View();
        }

        // Show all study records
        public IActionResult AllRecords()
        {
            return View(_dataService.StudyRecords);
        }

        // GET: Show form to select a student
        [HttpGet]
        public IActionResult SelectStudent()
        {
            ViewBag.Students = _dataService.Students;
            return View();
        }

        // GET: Show form to select a subject type
        [HttpGet]
        public IActionResult SelectSubject()
        {
            // Note: StudyRecord model changed, no longer has Subject field
            ViewBag.Subjects = new List<string>();
            return View();
        }

        // Show study records by subject
        public IActionResult BySubject(string subject)
        {
            // Note: StudyRecord model changed, no longer has Subject field
            var records = new List<StudyRecord>();
            ViewBag.Subject = subject;
            return View(records);
        }

        // Show study records for a specific student
        public IActionResult ByStudent(string firstName)
        {
            var studentRecords = _dataService.StudyRecords
                .Where(r => r.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewBag.StudentName = firstName;

            return View(studentRecords);
        }

        // GET: Show the form to create a new study record
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Receive the form data and save it
        [HttpPost]
        public IActionResult Create(StudyRecord record)
        {
            // Generate a new ID
            record.Id = _dataService.StudyRecords.Count > 0
                ? _dataService.StudyRecords.Max(r => r.Id) + 1
                : 1;

            // Set the created date
            record.CreatedDate = DateTime.Now;

            // Add to our list
            _dataService.StudyRecords.Add(record);

            // Redirect to the index page
            TempData["SuccessMessage"] = "Study record created successfully!";
            return RedirectToAction("Index");
        }
    }
}