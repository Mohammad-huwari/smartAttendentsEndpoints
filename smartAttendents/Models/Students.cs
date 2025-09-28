using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAttendents.Models
{
    public class Students
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }
        public string? UniversityID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? FaceID { get; set; }
        public bool? IsActive { get; set; }
    }

}
