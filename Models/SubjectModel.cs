using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace SpecShare.Models
{
    public class SubjectModel
    {

        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;

        [ForeignKey("DepartmentModel")]
        public int DepartmentId { get; set; }

        public int Semester { get; set; }

        public string SubjectName { get; set; } = string.Empty;
    }
}
