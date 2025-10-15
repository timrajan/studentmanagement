using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class TeamController : BaseController
    {
        private readonly DataService _dataService;

        public TeamController(DataService dataService)
        {
            _dataService = dataService;
        }

        // GET: Show all teams
        public IActionResult Index()
        {
            // We need to pass BOTH teams and their admins/students to the view
            // We'll use ViewBag for the admins and students
            ViewBag.TeamAdmins = _dataService.TeamAdmins;
            ViewBag.Students = _dataService.Students;

            // Get the current user's role and username
            var role = ViewBag.Role as string;
            var currentUsername = Environment.UserName;

            // If TeamAdmin, only show their own team
            if (role == "TeamAdmin")
            {
                // Find the team(s) that this admin manages
                var adminRecord = _dataService.TeamAdmins
                    .FirstOrDefault(ta => ta.Username.Equals(currentUsername, StringComparison.OrdinalIgnoreCase));

                if (adminRecord != null)
                {
                    // Return only the teams this admin manages
                    var adminTeams = _dataService.Teams.Where(t => t.Id == adminRecord.TeamId).ToList();
                    return View(adminTeams);
                }
            }

            // For SuperAdmin or if no admin record found, show all teams
            return View(_dataService.Teams);
        }

        // GET: Show the form to create a new team
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Receive the form data and create the team
        [HttpPost]
        public IActionResult Create(Team team, string adminName, string adminEmail)
        {
            // Validate team data
            if (string.IsNullOrEmpty(team.Name))
            {
                ViewBag.Error = "Team name is required.";
                return View(team);
            }

            // Validate at least one admin
            if (string.IsNullOrEmpty(adminName))
            {
                ViewBag.Error = "Admin name is required.";
                return View(team);
            }

            // Generate a new team ID
            team.Id = _dataService.Teams.Count > 0
                ? _dataService.Teams.Max(t => t.Id) + 1
                : 1;

            // Set default description if not provided
            if (string.IsNullOrEmpty(team.Description))
            {
                team.Description = $"Team managed by {adminName}";
            }

            // Set the created date
            team.CreatedDate = DateTime.Now;

            // Add the team
            _dataService.Teams.Add(team);

            // Create the first Team Admin
            var newAdmin = new TeamAdmin
            {
                Id = _dataService.TeamAdmins.Count > 0
                    ? _dataService.TeamAdmins.Max(a => a.Id) + 1
                    : 1,
                TeamId = team.Id,
                Name = adminName,
                Email = string.IsNullOrEmpty(adminEmail) ? $"{adminName.Replace(" ", "").ToLower()}@school.com" : adminEmail,
                Username = "", // No Windows username by default
                AddedDate = DateTime.Now
            };

            _dataService.TeamAdmins.Add(newAdmin);

            // Show success message
            TempData["SuccessMessage"] = $"Team '{team.Name}' has been created with {adminName} as Team Admin!";

            // Redirect to the teams list
            return RedirectToAction("Index");
        }

        // GET: Show form to add another admin to an existing team
        [HttpGet]
        public IActionResult AddAdmin(int teamId)
        {
            var team = _dataService.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            ViewBag.Team = team;
            return View();
        }

        // POST: Add a new admin to a team
        [HttpPost]
        public IActionResult AddAdmin(int teamId, string adminName, string adminEmail)
        {
            var team = _dataService.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            // Validate
            if (string.IsNullOrEmpty(adminName) || string.IsNullOrEmpty(adminEmail))
            {
                ViewBag.Error = "Admin name and email are required.";
                ViewBag.Team = team;
                return View();
            }

            // Create the new admin
            var newAdmin = new TeamAdmin
            {
                Id = _dataService.TeamAdmins.Count > 0
                    ? _dataService.TeamAdmins.Max(a => a.Id) + 1
                    : 1,
                TeamId = teamId,
                Name = adminName,
                Email = adminEmail,
                Username = "", // No Windows username by default
                AddedDate = DateTime.Now
            };

            _dataService.TeamAdmins.Add(newAdmin);

            TempData["SuccessMessage"] = $"{adminName} has been added as a Team Admin for '{team.Name}'!";

            return RedirectToAction("Index");
        }

        // GET: Show form to remove a team
        [HttpGet]
        public IActionResult Remove()
        {
            return View();
        }

        // POST: Remove a team by name
        [HttpPost]
        public IActionResult Remove(string teamName)
        {
            if (string.IsNullOrEmpty(teamName))
            {
                ViewBag.Error = "Team name is required.";
                return View();
            }

            var team = _dataService.Teams.FirstOrDefault(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase));

            if (team == null)
            {
                ViewBag.Error = $"Team '{teamName}' not found.";
                return View();
            }

            // Remove all admins associated with this team
            var adminsToRemove = _dataService.TeamAdmins.Where(a => a.TeamId == team.Id).ToList();
            foreach (var admin in adminsToRemove)
            {
                _dataService.TeamAdmins.Remove(admin);
            }

            // Remove the team
            _dataService.Teams.Remove(team);

            TempData["SuccessMessage"] = $"Team '{teamName}' has been removed successfully!";
            return RedirectToAction("Index");
        }

        // GET: Show form to add a team member
        [HttpGet]
        public IActionResult AddMember()
        {
            return View();
        }

        // POST: Add a new team member (student)
        [HttpPost]
        public IActionResult AddMember(string name, string createAccess)
        {
            // Validate
            if (string.IsNullOrEmpty(name))
            {
                ViewBag.Error = "Name is required.";
                return View();
            }

            // Get the current Windows username
            var currentUsername = Environment.UserName;

            // Find the team(s) that this admin manages
            var adminRecord = _dataService.TeamAdmins
                .FirstOrDefault(ta => ta.Username.Equals(currentUsername, StringComparison.OrdinalIgnoreCase));

            if (adminRecord == null)
            {
                ViewBag.Error = "You are not authorized to add team members. No team admin record found for your username.";
                return View();
            }

            // Get the team
            var team = _dataService.Teams.FirstOrDefault(t => t.Id == adminRecord.TeamId);
            if (team == null)
            {
                ViewBag.Error = "Your assigned team could not be found.";
                return View();
            }

            // Create the new student (team member) - only for the admin's team
            var newStudent = new Student
            {
                Id = _dataService.Students.Count > 0
                    ? _dataService.Students.Max(s => s.Id) + 1
                    : 1,
                Name = name,
                Email = $"{name.Replace(" ", "").ToLower()}@school.com",
                TeamId = team.Id,
                EnrollmentDate = DateTime.Now,
                Role = createAccess == "Yes" ? "Creator" : "Viewer"
            };

            _dataService.Students.Add(newStudent);

            TempData["SuccessMessage"] = $"{name} has been added to {team.Name} with {(createAccess == "Yes" ? "Creator" : "Viewer")} access!";

            return RedirectToAction("Index");
        }

        // GET: Show form to remove a team member
        [HttpGet]
        public IActionResult RemoveMember()
        {
            return View();
        }

        // POST: Remove a team member by name
        [HttpPost]
        public IActionResult RemoveMember(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ViewBag.Error = "Name is required.";
                return View();
            }

            // Get the current Windows username
            var currentUsername = Environment.UserName;

            // Find the team(s) that this admin manages
            var adminRecord = _dataService.TeamAdmins
                .FirstOrDefault(ta => ta.Username.Equals(currentUsername, StringComparison.OrdinalIgnoreCase));

            if (adminRecord == null)
            {
                ViewBag.Error = "You are not authorized to remove team members. No team admin record found for your username.";
                return View();
            }

            // Find the student
            var student = _dataService.Students.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (student == null)
            {
                ViewBag.Error = $"Team member '{name}' not found.";
                return View();
            }

            // Verify the student belongs to the admin's team
            if (student.TeamId != adminRecord.TeamId)
            {
                ViewBag.Error = $"You can only remove members from your own team. '{name}' belongs to a different team.";
                return View();
            }

            // Remove all study records for this student
            var studyRecordsToRemove = _dataService.StudyRecords.Where(sr => sr.StudentId == student.Id).ToList();
            foreach (var record in studyRecordsToRemove)
            {
                _dataService.StudyRecords.Remove(record);
            }

            // Remove all sports records for this student
            var sportsRecordsToRemove = _dataService.SportsRecords.Where(sr => sr.StudentId == student.Id).ToList();
            foreach (var record in sportsRecordsToRemove)
            {
                _dataService.SportsRecords.Remove(record);
            }

            // Remove the student
            _dataService.Students.Remove(student);

            TempData["SuccessMessage"] = $"Team member '{name}' has been removed successfully!";
            return RedirectToAction("Index");
        }
    }
}