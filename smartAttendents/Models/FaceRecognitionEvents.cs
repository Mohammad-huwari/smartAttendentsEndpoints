using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAttendents.Models
{
    public class FaceRecognitionEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }
        public string? SAFRPersonID { get; set; }
        public int? StudentID { get; set; }
        public string? ImagePath { get; set; }

        
        public double? ConfidenceScore { get; set; }

        public DateTime? DetectedTime { get; set; }
        public bool? Matched { get; set; }
        public int? ClassID { get; set; }
    }

}
