using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace smartAttendents.Models
{
   
    public class AttendanceLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceID { get; set; }
        public int? ClassID { get; set; }
        public int? StudentID { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string? AttendanceStatus { get; set; }
        public string? Source { get; set; }
        public bool? SyncedToMoodle { get; set; }
        public DateTime? SyncedTime { get; set; }

        [Timestamp] public byte[]? RowVersion { get; set; }

      
    }
}
