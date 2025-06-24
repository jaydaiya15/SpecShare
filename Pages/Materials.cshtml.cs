using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;

namespace SpecShare.Pages;

public class MaterialsModel : PageModel
{
    private readonly AppDbContext _db;
    public MaterialsModel (AppDbContext db)
    {
        _db = db;
    }

    public List<DepartmentModel> DepartmentsList { get; set; } = new();
    public List<MaterialMoodel> MaterialsList { get; set; } = new();
    public List<string> AvailableMaterialTypes { get; set; } = new();
    public List<string> AvailableSubjectCodes { get; set; } = new();
    public Dictionary<int, string> UploadedByNames { get; set; } = new(); 

    [BindProperty] public string departmentCode { get; set; } = string.Empty;
    [BindProperty] public int semester{ get; set; }
    [BindProperty] public string SelectedMaterialType { get; set; } = string.Empty;
    [BindProperty] public string SelectedSubjectCode { get; set; } = string.Empty;
    public Studentt LoggedInStudent { get; set; } = new();
    public int? StudentID { get; set; }


    public void LoadCommonData()
    {
        StudentID = HttpContext.Session.GetInt32("StudentID");

        if (StudentID != null)
        {
            LoggedInStudent = _db.StudentsData.FirstOrDefault(s => s.Id == StudentID) ?? new Studentt();
        }

        DepartmentsList = _db.DepartmentsData.ToList();
    }
    public void OnGet()
    {
        LoadCommonData();
        
    }
    public IActionResult OnPost() {

        LoadCommonData();
        MaterialsList = _db.Materials
               .Where(m => m.DepartmentCode == departmentCode && m.Semester == semester)
               .OrderByDescending(m => m.UploadedOn)
               .ToList();

        UploadedByNames = _db.StudentsData
            .Where(s => MaterialsList.Select(m => m.UploadedBy).Contains(s.Id))
            .ToDictionary(s => s.Id, s => $"{s.FirstName} {s.LastName}");


        var baseQuery = _db.Materials
            .Where(m => m.DepartmentCode == departmentCode && m.Semester == semester);


        AvailableMaterialTypes = baseQuery.Select(m => m.MaterialType).Distinct().ToList();
        AvailableSubjectCodes = baseQuery.Select(m => m.SubjectCode).Distinct().ToList();

        MaterialsList = baseQuery.OrderByDescending(m => m.UploadedOn).ToList();

        return Page();
    }

    public IActionResult OnPostFiltered()
    {
        DepartmentsList = _db.DepartmentsData.ToList();

        var query = _db.Materials
            .Where(m => m.DepartmentCode == departmentCode && m.Semester == semester);

        // Preserve filter dropdowns
        AvailableMaterialTypes = query.Select(m => m.MaterialType).Distinct().ToList();
        AvailableSubjectCodes = query.Select(m => m.SubjectCode).Distinct().ToList();

        if (!string.IsNullOrEmpty(SelectedMaterialType))
            query = query.Where(m => m.MaterialType == SelectedMaterialType);

        if (!string.IsNullOrEmpty(SelectedSubjectCode))
            query = query.Where(m => m.SubjectCode == SelectedSubjectCode);

        MaterialsList = query.OrderByDescending(m => m.UploadedOn).ToList();
        UploadedByNames = _db.StudentsData
            .Where(s => MaterialsList.Select(m => m.UploadedBy).Contains(s.Id))
            .ToDictionary(s => s.Id, s => $"{s.FirstName} {s.LastName}");


        return Page();
    }
}

