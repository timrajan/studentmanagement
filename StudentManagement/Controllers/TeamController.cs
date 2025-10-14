using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class TeamController : Controller
    {
        // Our "database" of teams
        private static List<Team> teams = new List<Team>
        {
            new Team 
            { 
                Id = 1, 
                Name = "Alpha Team", 
                Description = "Frontend Development Team",
                CreatedDate = DateTime.Now.AddMonths(-6)
            },
            new Team 
            { 
                Id = 2, 
                Name = "Beta Team", 
                Description = "Backend Development Team",
                CreatedDate = DateTime.Now.AddMonths(-4)
            }
        };

        // Our "database" of team admins
        private static List<TeamAdmin> teamAdmins = new List<TeamAdmin>
        {
            new TeamAdmin
            {
                Id = 1,
                TeamId = 1,  // Alpha Team
                Name = "Sarah Johnson",
                Email = "sarah@school.com",
                AddedDate = DateTime.Now.AddMonths(-6)
            },
            new TeamAdmin
            {
                Id = 2,
                TeamId = 1,  // Alpha Team (second admin!)
                Name = "David Lee",
                Email = "david@school.com",
                AddedDate = DateTime.Now.AddMonths(-5)
            },
            new TeamAdmin
            {
                Id = 3,
                TeamId = 2,  // Beta Team
                Name = "Michael Chen",
                Email = "michael@school.com",
                AddedDate = DateTime.Now.AddMonths(-4)
            }
        };

        // GET: Show all teams
        public IActionResult Index()
        {
            // We need to pass BOTH teams and their admins to the view
            // We'll use ViewBag for the admins
            ViewBag.TeamAdmins = teamAdmins;
            return View(teams);
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
            if (string.IsNullOrEmpty(team.Name) || 
                string.IsNullOrEmpty(team.Description))
            {
                ViewBag.Error = "Team name and description are required.";
                return View(team);
            }
            
            // Validate at least one admin
            if (string.IsNullOrEmpty(adminName) || string.IsNullOrEmpty(adminEmail))
            {
                ViewBag.Error = "At least one Team Admin is required.";
                return View(team);
            }
            
            // Generate a new team ID
            team.Id = teams.Count > 0 
                ? teams.Max(t => t.Id) + 1 
                : 1;
            
            // Set the created date
            team.CreatedDate = DateTime.Now;
            
            // Add the team
            teams.Add(team);
            
            // Create the first Team Admin
            var newAdmin = new TeamAdmin
            {
                Id = teamAdmins.Count > 0 
                    ? teamAdmins.Max(a => a.Id) + 1 
                    : 1,
                TeamId = team.Id,
                Name = adminName,
                Email = adminEmail,
                AddedDate = DateTime.Now
            };
            
            teamAdmins.Add(newAdmin);
            
            // Show success message
            TempData["SuccessMessage"] = $"Team '{team.Name}' has been created with {adminName} as Team Admin!";
            
            // Redirect to the teams list
            return RedirectToAction("Index");
        }

        // GET: Show form to add another admin to an existing team
        [HttpGet]
        public IActionResult AddAdmin(int teamId)
        {
            var team = teams.FirstOrDefault(t => t.Id == teamId);
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
            var team = teams.FirstOrDefault(t => t.Id == teamId);
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
                Id = teamAdmins.Count > 0 
                    ? teamAdmins.Max(a => a.Id) + 1 
                    : 1,
                TeamId = teamId,
                Name = adminName,
                Email = adminEmail,
                AddedDate = DateTime.Now
            };
            
            teamAdmins.Add(newAdmin);
            
            TempData["SuccessMessage"] = $"{adminName} has been added as a Team Admin for '{team.Name}'!";
            
            return RedirectToAction("Index");
        }
    }
}