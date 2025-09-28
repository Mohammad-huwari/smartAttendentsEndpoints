using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace smartAttendents.Models
{
    
    public class ClassEnrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }
        public int? ClassID { get; set; }
        public int? StudentID { get; set; }

        [Timestamp] public byte[]? RowVersion { get; set; }
    }

}
