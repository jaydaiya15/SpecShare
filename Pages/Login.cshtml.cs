using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;

namespace SpecShare.Pages
{
    public class LoginModel : PageModel
    {

        private readonly AppDbContext _db;
        public LoginModel (AppDbContext db) {
            _db = db;
        }

        [BindProperty]
        public Studentt LoginStudent { get; set; } = new();
     


        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var student = _db.StudentsData
                .FirstOrDefault(s => s.Enrollment == LoginStudent.Enrollment);

            if (student != null && BCrypt.Net.BCrypt.Verify(LoginStudent.Password, student.Password))
            {
                // Store data in session
                HttpContext.Session.SetString("StudentName", student.FirstName);
                HttpContext.Session.SetInt32("StudentID", student.Id);
                HttpContext.Session.SetString("IsLoggedIn", "true");


                TempData["LoginSuccess"] = true;
                return Page(); // Create this later
            }

            ErrorMessage = "Invalid credentials. Please try again.";
            return Page();
        }
    }
}
