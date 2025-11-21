using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;

namespace POE_CMCS_Mvc.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly IConfiguration _config;

        public HRController(IConfiguration config)
        {
            _config = config;
        }

        private string Conn => _config.GetConnectionString("DefaultConnection");

        // HR Dashboard showing approved claims
        public IActionResult Index()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(Conn))
            {
                string query = @"SELECT ClaimId, LecturerUsername, ModuleName, HoursWorked, HourlyRate, 
                                 (HoursWorked * HourlyRate) AS TotalAmount, DateSubmitted
                                 FROM Claims
                                 WHERE Status = 'Final Approved for Payment'";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.Fill(dt);
                }
            }

            return View(dt);
        }

        // Export CSV
        [HttpPost]
        public FileResult ExportCsv()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(Conn))
            {
                string query = @"SELECT LecturerUsername, COUNT(*) AS ClaimCount, 
                                SUM(HoursWorked) AS TotalHours,
                                SUM(HoursWorked * HourlyRate) AS TotalAmount
                                FROM Claims
                                WHERE Status = 'Final Approved for Payment'
                                GROUP BY LecturerUsername";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    da.Fill(dt);
                }
            }

            // Build CSV
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Lecturer,ClaimCount,TotalHours,TotalAmount");

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine($"{row["LecturerUsername"]},{row["ClaimCount"]},{row["TotalHours"]},{row["TotalAmount"]}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "PaymentReport.csv");
        }
    }
}
