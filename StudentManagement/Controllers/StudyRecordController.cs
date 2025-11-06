using Microsoft.AspNetCore.Mvc;
using StudentManagement.Data;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class StudyRecordController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureDevOpsService _azureDevOpsService;

        public StudyRecordController(ApplicationDbContext context, AzureDevOpsService azureDevOpsService)
        {
            _context = context;
            _azureDevOpsService = azureDevOpsService;
        }

        // Show the main study record page with buttons
        public IActionResult Index()
        {
            return View();
        }

        // API endpoint to get the next available study record ID
        [HttpGet]
        public JsonResult GetNextStudyRecordId()
        {
            var maxId = _context.StudyRecords.Max(r => (int?)r.Id) ?? 0;
            var nextId = maxId + 1;
            return Json(new { nextId = nextId });
        }

        // Webhook endpoint for Azure DevOps to notify build completion
        [HttpPost]
        [Route("StudyRecord/BuildWebhook")]
        public IActionResult BuildWebhook([FromBody] dynamic payload)
        {
            try
            {
                // Log the incoming webhook
                Console.WriteLine($"Webhook received: {payload}");

                // Extract build information from Azure DevOps webhook payload
                // The exact structure depends on your Azure DevOps webhook configuration
                // Common structure: payload.resource.status and payload.resource.result

                string buildStatus = payload?.resource?.status?.ToString();
                string buildResult = payload?.resource?.result?.ToString();

                // Get custom data passed in the build (e.g., study record ID)
                // You'll need to pass the record ID when triggering the build
                string recordIdStr = payload?.resource?.templateParameters?.recordId?.ToString();

                if (string.IsNullOrEmpty(recordIdStr))
                {
                    return BadRequest("Record ID not found in webhook payload");
                }

                if (!int.TryParse(recordIdStr, out int recordId))
                {
                    return BadRequest("Invalid record ID");
                }

                // Find the study record
                var record = _context.StudyRecords.FirstOrDefault(r => r.Id == recordId);
                if (record == null)
                {
                    return NotFound($"Study record with ID {recordId} not found");
                }

                // Update status based on build result
                if (buildStatus == "completed")
                {
                    if (buildResult == "succeeded")
                    {
                        record.Status = "Success";
                    }
                    else if (buildResult == "failed" || buildResult == "canceled")
                    {
                        record.Status = "Fail";
                    }

                    _context.SaveChanges();
                    Console.WriteLine($"Updated record {recordId} status to: {record.Status}");
                }

                return Ok(new { message = "Webhook processed successfully", recordId = recordId, status = record.Status });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing webhook: {ex.Message}");
                return StatusCode(500, $"Error processing webhook: {ex.Message}");
            }
        }

        // Show all study records
        public IActionResult AllRecords()
        {
            var records = _context.StudyRecords.ToList();
            return View(records);
        }

        // GET: Show form to select a student
        [HttpGet]
        public IActionResult SelectStudent()
        {
            var students = _context.Students.ToList();
            ViewBag.Students = students;
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
            var studentRecords = _context.StudyRecords
                .ToList()
                .Where(r => r.FirstName != null && r.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewBag.StudentName = firstName;

            return View(studentRecords);
        }

        // GET: Show the form to create a new study record
        [HttpGet]
        public IActionResult Create()
        {
            // Get current user's Windows username
            string username = ViewBag.Username?.ToString() ?? Environment.UserName;

            // Find the TeamAdmin record for this user - Load to memory first, then filter
            var teamAdmin = _context.TeamAdmins
                .ToList()
                .FirstOrDefault(ta => ta.Username != null && ta.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (teamAdmin != null)
            {
                // Get the actual team name from the database
                var team = _context.Teams.FirstOrDefault(t => t.Id == teamAdmin.TeamId);
                ViewBag.UserTeam = team?.Name ?? "teamA";
            }
            else
            {
                // Default to first team if user not found
                var defaultTeam = _context.Teams.FirstOrDefault();
                ViewBag.UserTeam = defaultTeam?.Name ?? "teamA";
            }

            return View();
        }

        // POST: Receive the form data and save it
        [HttpPost]
        public IActionResult Create(StudyRecord record)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(record.Team) ||
                string.IsNullOrWhiteSpace(record.Environment) ||
                string.IsNullOrWhiteSpace(record.FirstName) ||
                string.IsNullOrWhiteSpace(record.StudentIQLevel) ||
                string.IsNullOrWhiteSpace(record.MiddleName) ||
                string.IsNullOrWhiteSpace(record.StudentRollNumber) ||
                string.IsNullOrWhiteSpace(record.LastName) ||
                string.IsNullOrWhiteSpace(record.StudentRollName) ||
                string.IsNullOrWhiteSpace(record.DateOfBirth) ||
                string.IsNullOrWhiteSpace(record.StudentParentEmailAddress) ||
                string.IsNullOrWhiteSpace(record.EmailAddress) ||
                string.IsNullOrWhiteSpace(record.Status) ||
                string.IsNullOrWhiteSpace(record.StudentIdentityID) ||
                string.IsNullOrWhiteSpace(record.Type) ||
                string.IsNullOrWhiteSpace(record.StudentInitialID) ||
                string.IsNullOrWhiteSpace(record.Tags) ||
                string.IsNullOrWhiteSpace(record.Release) ||
                string.IsNullOrWhiteSpace(record.Comments))
            {
                TempData["ErrorMessage"] = "Missing Mandatory study record data";

                // Get user's team again for the view
                string username = ViewBag.Username?.ToString() ?? Environment.UserName;
                var teamAdmin = _context.TeamAdmins
                    .ToList()
                    .FirstOrDefault(ta => ta.Username != null && ta.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (teamAdmin != null)
                {
                    // Get the actual team name from the database
                    var team = _context.Teams.FirstOrDefault(t => t.Id == teamAdmin.TeamId);
                    ViewBag.UserTeam = team?.Name ?? "teamA";
                }
                else
                {
                    // Default to first team if user not found
                    var defaultTeam = _context.Teams.FirstOrDefault();
                    ViewBag.UserTeam = defaultTeam?.Name ?? "teamA";
                }

                return View(record);
            }

            // Set the created date
            record.CreatedDate = DateTime.UtcNow;

            // Add to database first to get the auto-generated ID
            _context.StudyRecords.Add(record);
            _context.SaveChanges();

            // Now generate the email address using the actual assigned ID
            var formattedId = record.Id.ToString("D3"); // Format with leading zeros (001, 002, etc.)
            record.EmailAddress = $"{record.Team}-{record.Environment}-{record.Type}-{formattedId}@gmail.com";

            // Update the record with the generated email
            _context.SaveChanges();

            // Trigger Azure DevOps Build Pipeline with captured values
            var (success, message) = _azureDevOpsService.TriggerBuildPipeline(record);

            if (success)
            {
                // Update status to InProgress since build was triggered successfully
                record.Status = "InProgress";
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Study record created and build pipeline triggered. Status: InProgress";
            }
            else
            {
                // Build trigger failed, keep status as is or set to error
                TempData["ErrorMessage"] = message;
            }

            // Redirect to the index page
            return RedirectToAction("Index");
        }

        // GET: Show the View Students page
        [HttpGet]
        public IActionResult ViewStudents()
        {
            // Get current user's Windows username
            string username = ViewBag.Username?.ToString() ?? Environment.UserName;

            // Find the TeamAdmin record for this user - Load to memory first, then filter
            var teamAdmin = _context.TeamAdmins
                .ToList()
                .FirstOrDefault(ta => ta.Username != null && ta.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (teamAdmin != null)
            {
                // Get the actual team name from the database
                var team = _context.Teams.FirstOrDefault(t => t.Id == teamAdmin.TeamId);
                ViewBag.UserTeam = team?.Name ?? "teamA";
            }
            else
            {
                // Default to first team if user not found
                var defaultTeam = _context.Teams.FirstOrDefault();
                ViewBag.UserTeam = defaultTeam?.Name ?? "teamA";
            }

            return View();
        }

        // POST: Handle the View button click
        [HttpPost]
        public IActionResult ViewStudents(string filterType, string filterValue)
        {
            // Get current user's team info (same as GET action)
            string username = ViewBag.Username?.ToString() ?? Environment.UserName;

            // Load to memory first, then filter
            var teamAdmin = _context.TeamAdmins
                .ToList()
                .FirstOrDefault(ta => ta.Username != null && ta.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (teamAdmin != null)
            {
                // Get the actual team name from the database
                var team = _context.Teams.FirstOrDefault(t => t.Id == teamAdmin.TeamId);
                ViewBag.UserTeam = team?.Name ?? "teamA";
            }
            else
            {
                // Default to first team if user not found
                var defaultTeam = _context.Teams.FirstOrDefault();
                ViewBag.UserTeam = defaultTeam?.Name ?? "teamA";
            }

            // Query the database based on filterType and filterValue
            if (!string.IsNullOrEmpty(filterType) && !string.IsNullOrEmpty(filterValue))
            {
                var allRecords = _context.StudyRecords.ToList();
                List<StudyRecord> results = new List<StudyRecord>();

                switch (filterType)
                {
                    case "Team":
                        results = allRecords
                            .Where(r => r.Team != null && r.Team.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "Environment":
                        results = allRecords
                            .Where(r => r.Environment != null && r.Environment.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "RollNumber":
                        results = allRecords
                            .Where(r => r.StudentRollNumber != null && r.StudentRollNumber.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "EmailAddress":
                        results = allRecords
                            .Where(r => r.EmailAddress != null && r.EmailAddress.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }

                ViewBag.Results = results;
                ViewBag.FilterType = filterType;
                ViewBag.FilterValue = filterValue;
            }

            return View();
        }

        // GET: Show the Edit page for a study record
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var record = _context.StudyRecords.ToList().FirstOrDefault(r => r.Id == id);
            if (record == null)
            {
                return NotFound();
            }
            return View(record);
        }

        // POST: Update only the Comments field
        [HttpPost]
        public IActionResult Edit(int id, string Comments)
        {
            var record = _context.StudyRecords.ToList().FirstOrDefault(r => r.Id == id);
            if (record == null)
            {
                return NotFound();
            }

            // Update only the Comments field
            record.Comments = Comments;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Study record updated successfully!";
            return RedirectToAction("ViewStudents");
        }
    }
}
