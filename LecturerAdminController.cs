using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;

namespace POE_CMCS_Mvc.Controllers
{
    [Authorize(Roles = "HR")]
    public class LecturerAdminController : Controller
    {
        private readonly IConfiguration _config;

        public LecturerAdminController(IConfiguration config)
        {
            _config = config;
        }

        private string Conn => _config.GetConnectionString("DefaultConnection");

        public IActionResult Index()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(Conn))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Lecturers", con);
                da.Fill(dt);
            }

            return View(dt);
        }

        public IActionResult Edit(int id)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(Conn))
            {
                SqlDataAdapter da =
                    new SqlDataAdapter("SELECT * FROM Lecturers WHERE LecturerId=" + id, con);
                da.Fill(dt);
            }

            return View(dt.Rows[0]);
        }

        [HttpPost]
        public IActionResult Edit(int LecturerId, string FirstName, string LastName, string Email, string Phone, string Address)
        {
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Lecturers SET 
                      FirstName=@FirstName, LastName=@LastName,
                      Email=@Email, Phone=@Phone, Address=@Address
                      WHERE LecturerId=@LecturerId", con);

                cmd.Parameters.AddWithValue("@LecturerId", LecturerId);
                cmd.Parameters.AddWithValue("@FirstName", FirstName);
                cmd.Parameters.AddWithValue("@LastName", LastName);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Phone", Phone);
                cmd.Parameters.AddWithValue("@Address", Address);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
