using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class StudyRecordController : Controller
    {
        // Fake "database" of study records
        private static List<StudyRecord> studyRecords = new List<StudyRecord>
        {
            new StudyRecord
            {
                Id = 1,
                StudentId = 1,  // Alice John
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
                StudentId = 1,  // Alice John again
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
                StudentId = 2,  // Tim Alex
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
                StudentId = 3,  // Rah Man
                Subject = "Computer Science",
                Topic = "Data Structures",
                HoursSpent = 2.0,
                Notes = "Studied binary trees and traversal algorithms.",
                StudyDate = DateTime.Now,
                CreatedDate = DateTime.Now
            }
        };

        // Reference to students (so we can check roles and show names)
        private static List<Student> students = new List<Student>
        {
            new Student { Id = 1, Name = "Alice John", Email = "alice@alice.com", TeamId = 1, Role = "Creator" },
            new Student { Id = 2, Name = "Tim Alex", Email = "tim@tim.com", TeamId = 2, Role = "Viewer" },
            new Student { Id = 3, Name = "Rah Man", Email = "rah@rah.com", TeamId = 2, Role = "Creator" }
        };

        // Show all study records
        public IActionResult Index()
        {
            return View(studyRecords);
        }

        // Show study records for a specific student
        public IActionResult ByStudent(int studentId)
        {
            var studentRecords = studyRecords
                .Where(r => r.StudentId == studentId)
                .ToList();
            
            var student = students.FirstOrDefault(s => s.Id == studentId);
            
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
                var student = students.FirstOrDefault(s => s.Id == studentId.Value);
                
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
            ViewBag.Students = students.Where(s => s.Role == "Creator").ToList();
            
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
                ViewBag.Students = students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }
            
            // IMPORTANT: Check if the student has Creator role
            var student = students.FirstOrDefault(s => s.Id == record.StudentId);
            
            if (student == null)
            {
                ViewBag.Error = "Selected student not found.";
                ViewBag.Students = students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }
            
            if (student.Role != "Creator")
            {
                ViewBag.Error = $"{student.Name} does not have permission to create study records. Only 'Creator' role can add records.";
                ViewBag.Students = students.Where(s => s.Role == "Creator").ToList();
                return View(record);
            }
            
            // Generate a new ID
            record.Id = studyRecords.Count > 0 
                ? studyRecords.Max(r => r.Id) + 1 
                : 1;
            
            // Set the created date
            record.CreatedDate = DateTime.Now;
            
            // If StudyDate wasn't set, use today
            if (record.StudyDate == DateTime.MinValue)
            {
                record.StudyDate = DateTime.Now;
            }
            
            // Add to our list
            studyRecords.Add(record);
            
            // Redirect to the index page
            TempData["SuccessMessage"] = $"Study record created successfully by {student.Name}!";
            return RedirectToAction("Index");
        }
    }
}