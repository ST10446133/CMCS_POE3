using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POE_CMCS_Mvc.Services;
namespace POE_CMCS_Mvc.Controllers
{
    [Authorize(Roles = "Academic Manager")]
    public class AcademicManagerController : Controller
    {
        private readonly ClaimService _service;
        public AcademicManagerController(ClaimService service) => _service = service;
        public IActionResult Index()
        {
            var claims = _service.GetAllClaims();
            return View(claims);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalApprove(int id)

        {

            _service.UpdateStatus(id, "Final Approved for Payment");
            TempData["Message"] = $"Claim {id} final approved for payment.";
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
            var contentType =
            LecturerControllerHelper.GetContentTypeFromFileName(claim.FileName);
            return File(claim.SupportingDoc, contentType, claim.FileName);
        }
    }
}