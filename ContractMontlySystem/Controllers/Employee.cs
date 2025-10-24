using ContractMontlySystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlySystem.Controllers
{
    public class Employee : Controller
    {
        [HttpGet]
        public IActionResult Lecture()
        {
            sql_connection connect = new sql_connection();
            connect.createClaimsTable();
            return View();
        }

        [HttpPost]
        public IActionResult Lecture(Claims user, IFormFile SupportingDocs)
        {
            sql_connection connect = new sql_connection();

            // Check model validation
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation error: " + error.ErrorMessage);
                }
                return View(user);
            }

            // Handle file upload
            string filePath = string.Empty;
            if (SupportingDocs != null && SupportingDocs.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsDirectory))
                    Directory.CreateDirectory(uploadsDirectory);

                var fileName = Path.GetFileName(SupportingDocs.FileName);
                var filePathServer = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePathServer, FileMode.Create))
                {
                    SupportingDocs.CopyTo(stream);
                }

                filePath = "/uploads/" + fileName;
            }

            // Debug log
            Console.WriteLine($"Submitting claim: {user.LectureName}, {user.LectureId}, {user.ModuleName}, {user.ClaimFrom}-{user.ClaimTo}, {user.HourlyWage}, {user.SessionHours}, FilePath={filePath}");

            // Save claim to database
            bool isClaimSaved = connect.StoreClaimIntoTable(
                user.LectureName,
                user.LectureId,
                user.ModuleName,
                user.ClaimFrom,
                user.ClaimTo,
                user.HourlyWage,
                user.SessionHours,
                filePath
            );

            if (isClaimSaved)
            {
                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("ViewClaim");
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while submitting your claim. Please try again.");
                return View(user);
            }
        }


        [HttpGet]
        public IActionResult pc()
        {
            sql_connection connect = new sql_connection();
            var claims = connect.GetAllClaims();
            return View(claims);
        }

        // POST: Update claim statuses
        [HttpPost]
        public IActionResult UpdateClaimStatus(Dictionary<string, string> UpdatedStatuses, Dictionary<string, string> Reasons)
        {
            sql_connection connect = new sql_connection();

            // Update statuses
            bool isUpdated = connect.UpdateClaimStatuses(UpdatedStatuses);

            if (isUpdated)
            {
                TempData["Message"] = "Claim statuses updated successfully.";
            }
            else
            {
                TempData["Message"] = "Error updating claim statuses.";
            }

            // Optional: handle reasons (log or save in a separate history table)
            if (Reasons != null)
            {
                foreach (var entry in Reasons)
                {
                    string claimId = entry.Key;
                    string reason = entry.Value;

                    if (!string.IsNullOrEmpty(reason))
                    {
                        Console.WriteLine($"Claim {claimId} - Rejection reason: {reason}");
                    }
                }
            }

            // Return updated claims to the view
            var updatedClaims = connect.GetAllClaims();
            return View("pc", updatedClaims);
        }
        [HttpGet]
        public IActionResult am()
        {
            sql_connection connect = new sql_connection();
            var claims = connect.GetAllClaims();
            return View(claims);
            
        }

        [HttpPost]
        public IActionResult UpdateFinalApproval(Dictionary<string, string> FinalStatuses, Dictionary<string, string> ManagerComments)
        {
            sql_connection connect = new sql_connection();

            // Update the same Claim_Status column with the AM's decision
            bool isUpdated = connect.UpdateClaimStatuses(FinalStatuses);

            TempData["Message"] = isUpdated
                ? "Final approvals updated successfully."
                : "Error updating final approvals.";

            // Optional: log manager comments
            if (ManagerComments != null)
            {
                foreach (var entry in ManagerComments)
                {
                    string claimId = entry.Key;
                    string comment = entry.Value;
                    if (!string.IsNullOrEmpty(comment))
                        Console.WriteLine($"Claim {claimId} - Manager comment: {comment}");
                }
            }

            var updatedClaims = connect.GetAllClaims(); // refresh data
            return View("am", updatedClaims);
        }


        public IActionResult ViewClaim()
        {
            sql_connection db = new sql_connection();
            List<Claims> claims = db.GetAllClaims();
            return View(claims);
        }
    }
}
