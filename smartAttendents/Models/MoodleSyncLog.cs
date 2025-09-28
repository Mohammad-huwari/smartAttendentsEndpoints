using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAttendents.Models
{
    public class MoodleSyncLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SyncID { get; set; }
        public int? AttendanceID { get; set; }
        public string? APIResponse { get; set; }
        public string? SyncStatus { get; set; }
        public DateTime? SyncTime { get; set; }
    }
}

