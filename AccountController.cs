using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
namespace POE_CMCS_Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string
        password, string selectedRole)
        {
            if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(selectedRole))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }
            string conn = _config.GetConnectionString("DefaultConnection");
            try
            {

                using SqlConnection connection = new(conn);
                await connection.OpenAsync();
                string query = "SELECT Role FROM Users WHERE Username = @u AND Password = @p";
                using SqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                object? roleObj = await cmd.ExecuteScalarAsync();
                if (roleObj == null)
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }
                string dbRole = roleObj.ToString()!.Trim();
                // Normalize roles (allow small DB differences)
                string normalizedDbRole = NormalizeRole(dbRole);
                string normalizedSelected = NormalizeRole(selectedRole);
                if (!string.Equals(normalizedDbRole, normalizedSelected, StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Error = $"Selected role '{selectedRole}' does not match account role '{dbRole}'.";
                    return View();
                }
                // Create authentication cookie
                var claims = new List<Claim>
                 {
                 new Claim(ClaimTypes.Name, username),
                 new Claim(ClaimTypes.Role, normalizedDbRole)
                 };
                var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
                // Redirect based on normalized role
                return normalizedDbRole switch
                {
                    "Lecturer" => RedirectToAction("Index", "Lecturer"),
                    "Programme Coordinator" => RedirectToAction("Index", "ProgrammeCoordinator"),
                    "Academic Manager" => RedirectToAction("Index", "AcademicManager"),
                    _ => RedirectToAction("Login")
                };
            }

            catch (Exception ex)
            {
                // Log or handle the error appropriately; for now show a friendly message
                ViewBag.Error = "Error connecting to database: " + ex.Message;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return string.Empty;
            role = role.Trim().ToLowerInvariant();
            if (role.Contains("lectur")) return "Lecturer";
            if (role.Contains("coord") || (role.Contains("programme") &&
            role.Contains("coordinator"))) return "Programme Coordinator";
            if (role.Contains("manager")) return "Academic Manager";
            // default: Title case
            return
            System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(role);
        }
    }
}
