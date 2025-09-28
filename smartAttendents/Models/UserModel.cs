using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAttendents.Models
{
    [Table("Users")]
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Username { get; set; } = default!;

        public string? DisplayName { get; set; }
        public string? Role { get; set; } = Auth.AppRoles.Student;

        [Required] public string? PasswordHash { get; set; }
        [Required] public string? PasswordSalt { get; set; }

        public string? PasswordPlain { get; set; }

        public int? StudentID { get; set; }
        public int? InstructorID { get; set; }

    }
}
