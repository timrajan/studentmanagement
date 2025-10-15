using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentManagement.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Get Windows username and set role for all pages
            var windowsUsername = Environment.UserName;
            var role = GetUserRoleFromFile(windowsUsername);
            ViewBag.Role = role;
            ViewBag.Username = windowsUsername;
        }

        private string GetUserRoleFromFile(string username)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "users.txt");

                if (!System.IO.File.Exists(filePath))
                {
                    return "TeamAdmin";
                }

                var lines = System.IO.File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                        continue;

                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var fileUsername = parts[0].Trim();
                        var fileRole = parts[1].Trim();

                        if (fileUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                        {
                            return fileRole;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If any error, default to TeamAdmin
            }

            return "TeamAdmin";
        }
    }
}
