using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SpecShare.Models
{
    public class DepartmentModel
    {


        public int Id { get; set; }
        public int DepartmentCode { get; set; }

        public string DepartmentName { get; set; } = string.Empty;


    }
}
