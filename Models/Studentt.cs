using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpecShare.Models
{
    public class Studentt
    {

        public int Id { get; set; }

        [Required]
        public long Enrollment { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;


        [Required]
        public int Semester { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; }  = string.Empty;



    }
}