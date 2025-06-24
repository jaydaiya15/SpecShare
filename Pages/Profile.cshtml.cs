using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;

namespace SpecShare.Pages
{
    public class ProfileModel : PageModel
    {

        private readonly AppDbContext _db;
        public ProfileModel(AppDbContext db)
        {
            _db = db;
        }

        public Studentt LoggedInStudent { get; set; } = new();
        public int? StudentID { get; set; }

        public void LoadCommonData()
        {
            StudentID = HttpContext.Session.GetInt32("StudentID");

            if (StudentID != null)
            {
                LoggedInStudent = _db.StudentsData.FirstOrDefault(s => s.Id == StudentID) ?? new Studentt();
            }


        }
        public IActionResult OnGet()
        {
            LoadCommonData();
            return Page();
        }
        public IActionResult OnPostEdit(int Id, string FirstName, string LastName, string Email, string Department, int Semester)
        {
            LoadCommonData();
            var student = _db.StudentsData.FirstOrDefault(s => s.Id == Id);
            if (student != null)
            {
                student.FirstName = FirstName;
                student.LastName = LastName;
                student.Email = Email;
                student.Department = Department;
                student.Semester = Semester;

                _db.SaveChanges();
                TempData["EditSuccess"] = true;
            }

            return RedirectToPage();
        }

    }
}
