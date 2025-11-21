using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POE_CMCS_Mvc.Services;
namespace POE_CMCS_Mvc.Controllers
{
    [Authorize(Roles = "Programme Coordinator")]
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly ClaimService _service;
        public ProgrammeCoordinatorController(ClaimService service) => _service = service;
        public IActionResult Index()
        {
            var claims = _service.GetAllClaims();
            return View(claims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            _service.UpdateStatus(id, "Approved");
            TempData["Message"] = $"Claim {id} approved.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            _service.UpdateStatus(id, "Rejected");
            TempData["Message"] = $"Claim {id} rejected.";
            return RedirectToAction("Index");
        }
        public IActionResult Download(int id)
        {
            var claim = _service.GetClaim(id);
            if (claim == null) return NotFound();
            if (claim.SupportingDoc == null ||
            string.IsNullOrWhiteSpace(claim.FileName))
                return Content("No file uploaded.");

            var contentType = LecturerControllerHelper.GetContentTypeFromFileName(claim.FileName);
            return File(claim.SupportingDoc, contentType, claim.FileName);
        }
    }
}