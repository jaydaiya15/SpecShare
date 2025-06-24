using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;


namespace SpecShare.Pages
{
    public class RegisterModel : PageModel
    {

        private readonly AppDbContext _db;
        public RegisterModel (AppDbContext db) {
            _db = db;
        }

        [BindProperty] public Studentt AddStudent { get; set; } = new();

        public List<DepartmentModel> DepartmentsList { get; set; } = new();


        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;

        public string ConfirmPasswordError { get; set; } = string.Empty;

        public void OnGet()
        {
            DepartmentsList = _db.DepartmentsData.ToList();
        }


      


        public IActionResult OnPost () {

            DepartmentsList = _db.DepartmentsData.ToList();

            if (AddStudent.Password != ConfirmPassword)
                {
                    ConfirmPasswordError = "Passwords do not match.";
                    return Page();
                }

            AddStudent.Password = BCrypt.Net.BCrypt.HashPassword(AddStudent.Password);

            _db.StudentsData.Add(AddStudent);
            _db.SaveChanges();

             TempData["RegisterSuccess"] = true;

            return Page();
        }
    }
}
