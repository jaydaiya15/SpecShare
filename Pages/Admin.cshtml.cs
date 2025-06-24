using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;


namespace SpecShare.Pages
{
    public class AdminModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdminModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string AdminUsername { get; set; }

        [BindProperty] public string AdminPassword { get; set; }

        [BindProperty] public string Action { get; set; } = string.Empty;

        [BindProperty] public string SubjectCode { get; set; }

        [BindProperty] public string SubjectName { get; set; }

        [BindProperty] public int DepartmentId { get; set; }

        [BindProperty] public int Semester { get; set; }
        [BindProperty] public int MaterialId { get; set; }


        public bool IsAuthenticated { get; set; } = false;
        public bool LoginFailed { get; set; } = false;
        public bool ShowStudents { get; set; } = false;
        public bool ShowDepartments { get; set; } = false;
        public bool ShowSubjects { get; set; } = false;
        public bool ShowMaterials { get; set; } = false;

        public List<Studentt>? Students { get; set; }
        public List<DepartmentModel>? Departments { get; set; }
        public List<SubjectModel>? Subjects { get; set; }
        public List<MaterialMoodel>? Materials { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
        
            if (string.IsNullOrEmpty(Action))
            {
                if (AdminUsername == "admin7004" && AdminPassword == "specshare@123")
                {
                    IsAuthenticated = true;
                }
                else
                {
                    LoginFailed = true;
                }
            }
            else
            {
        
                IsAuthenticated = true;

                if (Action == "students")
                {
                    ShowStudents = true;
                    Students = _context.StudentsData.ToList(); // Table name is plural, model is Studentt
                }
                else if (Action == "departments")
                {
                    ShowDepartments = true;
                    Departments = _context.DepartmentsData.ToList(); // Match your DbSet name
                }
                else if (Action == "subjects")
                {
                    ShowSubjects = true;
                    Subjects = _context.SubjectsData.ToList();
                }
                else if (Action == "addsubject")
                {
                    // Save new subject
                    var newSubject = new SubjectModel
                    {
                        SubjectCode = SubjectCode,
                        SubjectName = SubjectName,
                        DepartmentId = DepartmentId,
                        Semester = Semester
                    };

                    _context.SubjectsData.Add(newSubject);
                    _context.SaveChanges();

                    // Refresh table
                    ShowSubjects = true;
                    Subjects = _context.SubjectsData.ToList();
                }
                else if (Action == "materials")
                {
                    ShowMaterials = true;
                    Materials = _context.Materials.ToList();
                }
                else if (Action == "deletematerial")
                {
                    var material = _context.Materials.FirstOrDefault(m => m.Id == MaterialId);
                    if (material != null)
                    {
                        _context.Materials.Remove(material);
                        _context.SaveChanges();
                    }

                    // Refresh list
                    ShowMaterials = true;
                    Materials = _context.Materials.ToList();
                }

            }
        }
    }
}
