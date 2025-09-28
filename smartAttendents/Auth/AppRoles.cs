using smartAttendents.Models;

namespace smartAttendents.Auth
{
    public class AppRoles
    {
        public const string Student = "student";
        public const string Instructor = "instructor";
        public const string Admin = "admin";
        public static readonly string[] All = new[] {  Student, Instructor , Admin };
    }
}
