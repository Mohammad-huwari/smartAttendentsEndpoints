using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAttendents.Models
{
   
    public class Classes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ClassID { get; set; }  

        public string? CourseCode { get; set; }
        public int? InstructorID { get; set; }
        public DateTime? ClassTime { get; set; }
        public string? Room { get; set; }
        public string? MoodleCourseID { get; set; }
        public string? MoodleInstanceID { get; set; }

        [Timestamp] public byte[]? RowVersion { get; set; }
    }
}
