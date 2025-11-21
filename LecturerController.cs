using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POE_CMCS_Mvc.Services;
using POE_CMCS_Mvc.Models;
namespace POE_CMCS_Mvc.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly ClaimService _service;
        public LecturerController(ClaimService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            var username = User.Identity?.Name ?? string.Empty;
            var claims = new List<ClaimModel>();
            if (!string.IsNullOrWhiteSpace(username))
            {
                claims = _service.GetClaimsByLecturer(username);
            }
            return View(claims);
        }
        public IActionResult SubmitClaim()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(decimal hoursWorked, decimal hourlyRate, string? notes, IFormFile? supportingFile)
        {
            var username = User.Identity?.Name ?? string.Empty;

            if (hoursWorked <= 0 || hourlyRate <= 0)
            {
                TempData["Error"] = "Hours worked and hourly rate must be greater than zero.";
                return RedirectToAction("Index");
            }

            var model = new ClaimModel
            {
                LecturerUsername = username,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                TotalAmount = hoursWorked * hourlyRate,  // NEW AUTOMATION
                Notes = notes ?? string.Empty,
                DateSubmitted = DateTime.Now
            };


            if (supportingFile != null && supportingFile.Length > 0)
            {
                using var ms = new MemoryStream();
                supportingFile.CopyTo(ms);
                model.SupportingDoc = ms.ToArray();
                model.FileName = Path.GetFileName(supportingFile.FileName) ?? string.Empty;
            }

            try
            {
                _service.SubmitClaim(model);
                TempData["Message"] = "Claim submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error submitting claim: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
        public IActionResult Download(int id)
        {
            var claim = _service.GetClaim(id);
            if (claim == null) return NotFound();
            var username = User.Identity?.Name ?? string.Empty;
            if (!string.Equals(claim.LecturerUsername, username, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (claim.SupportingDoc == null || string.IsNullOrWhiteSpace(claim.FileName))
                return Content("No file uploaded for this claim.");
            var contentType = GetContentTypeFromFileName(claim.FileName);
            return File(claim.SupportingDoc, contentType, claim.FileName);
        }
        private static string GetContentTypeFromFileName(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream",
            };
        }

    }
}


