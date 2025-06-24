using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;

namespace SpecShare.Pages
{
    public class DashboardModel : PageModel
    {

        private readonly AppDbContext _db;
        public DashboardModel(AppDbContext db)
        {
            _db = db;
        }

        public List<DepartmentModel> DepartmentsList { get; set; } = new(); 
       
        public List<MaterialMoodel> UploadedDocuments { get; set; } = new();
        public Studentt LoggedInStudent { get; set; } = new();  
        public int? StudentID { get; set; }

        [BindProperty] public string departmentCode { get; set; } = string.Empty;
        [BindProperty] public int semester { get; set; }
        [BindProperty] public string subject { get; set; } = string.Empty;
        [BindProperty] public string material { get; set; } = string.Empty;
        [BindProperty] public IFormFile uploadedFile { get; set; } = default!;
        [BindProperty] public string title { get; set; } = string.Empty;
        [BindProperty] public string description { get; set; } = string.Empty;


        public void LoadCommonData() {
           
            StudentID = HttpContext.Session.GetInt32("StudentID");

            if (StudentID != null)
            {
                LoggedInStudent = _db.StudentsData.FirstOrDefault(s => s.Id == StudentID) ?? new Studentt();
                DepartmentsList = _db.DepartmentsData.ToList();
            }

        }
        public IActionResult OnGet()
        {
            LoadCommonData();
            UploadedDocuments = _db.Materials
              .Where(m => m.UploadedBy == StudentID)
              .OrderByDescending(m => m.UploadedOn)
              .ToList();
            return Page();
        }

        public IActionResult OnPostFetchSubjects(int departmentCode, int semester)
        {
            var subjects = _db.SubjectsData
                            .Where(s => s.DepartmentId == departmentCode && s.Semester == semester)
                            .Select(s => new {
                                s.SubjectCode,
                                s.SubjectName
                            })
                            .ToList();

            return new JsonResult(subjects);
        }

        public IActionResult OnPost()
        {
            LoadCommonData();
            if (uploadedFile != null)
            {
                var extension = Path.GetExtension(uploadedFile.FileName).ToLower();
                if (extension != ".pdf" && extension != ".docx")
                {
                    ModelState.AddModelError("uploadedFile", "Only PDF or DOCX files are allowed.");
                    return Page();
                }

                // ⚠️ Make sure subject and departmentCode are not null or empty
                if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(departmentCode) || string.IsNullOrWhiteSpace(material))
                {
                    ModelState.AddModelError("", "All fields are required.");
                    return Page();
                }

                

                // 🔧 Build path: wwwroot/uploads/material/department/branch/
                string folderPath = Path.Combine("wwwroot", "uploads", material.ToLower(), departmentCode, semester.ToString());
                Directory.CreateDirectory(folderPath); // creates folder if not exist


                // 🔧 Build final file name: [subject].[extension]
                string finalFileName = $"{subject}{extension}";  // If you prefer SubjectName, use it here

                // 🔧 Full path for saving file
                string fullFilePath = Path.Combine(folderPath, finalFileName);


                // ✅ Save the file
                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    uploadedFile.CopyTo(stream);
                }

                // ✅ Save relative path to DB (for web access)
                string dbPath = Path.Combine("/uploads", material.ToLower(), departmentCode, semester.ToString(), finalFileName)
                                    .Replace("\\", "/");

                _db.Materials.Add(new MaterialMoodel
                {
                    Title = title,
                    Description = description,  
                    FilePath = dbPath,
                    MaterialType = material,
                    DepartmentCode = departmentCode,
                    BranchCode = departmentCode,
                    Semester = semester,
                    SubjectCode = subject,
                    UploadedBy = StudentID,
                    UploadedOn = DateTime.Now
                });
                _db.SaveChanges();

                TempData["UploadSuccess"] = "File uploaded successfully!";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var material = _db.Materials.Find(id);
            if (material != null)
            {
                // Delete the file from the server
                string filePath = Path.Combine("wwwroot", material.FilePath.TrimStart('/').Replace("/", "\\"));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                // Remove from database
                _db.Materials.Remove(material);
                _db.SaveChanges();
                TempData["DeleteSuccess"] = "File deleted successfully!";
            }
            else
            {
                TempData["DeleteError"] = "File not found.";
            }
            return RedirectToPage();
        }
    }
}
