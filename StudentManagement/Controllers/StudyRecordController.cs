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

        // Show all study records
        public IActionResult Index()
        {
            return View(_dataService.StudyRecords);
        }

        // Show study records for a specific student
        public IActionResult ByStudent(int studentId)
        {
            var studentRecords = _dataService.StudyRecords
                .Where(r => r.StudentId == studentId)
                .ToList();

            var student = _dataService.Students.FirstOrDefault(s => s.Id == studentId);

            ViewBag.StudentName = student?.Name ?? "Unknown Student";
            ViewBag.StudentId = studentId;

            return View(studentRecords);
        }

        // GET: Show the form to create a new study record
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
                    TempData["ErrorMessage"] = $"{student.Name} does not have permission to create study records. Only students with 'Creator' role can add records.";
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
        public IActionResult Create(StudyRecord record)
        {
            // Validate the data
            if (string.IsNullOrEmpty(record.Subject) ||
                string.IsNullOrEmpty(record.Topic) ||
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
                ViewBag.Error = $"{student.Name} does not have permission to create study records. Only 'Creator' role can add records.";
                ViewBag.Students = _dataService.Students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }

            // Generate a new ID
            record.Id = _dataService.StudyRecords.Count > 0
                ? _dataService.StudyRecords.Max(r => r.Id) + 1
                : 1;

            // Set the created date
            record.CreatedDate = DateTime.Now;

            // If StudyDate wasn't set, use today
            if (record.StudyDate == DateTime.MinValue)
            {
                record.StudyDate = DateTime.Now;
            }

            // Add to our list
            _dataService.StudyRecords.Add(record);

            // Redirect to the index page
            TempData["SuccessMessage"] = $"Study record created successfully by {student.Name}!";
            return RedirectToAction("Index");
        }
    }
}