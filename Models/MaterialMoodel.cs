namespace SpecShare.Models
{
    public class MaterialMoodel
    {
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public int? UploadedBy { get; set; }
        public DateTime UploadedOn { get; set; }
    }

}
