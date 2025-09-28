namespace smartAttendents.Dtos
{
    public sealed class RegisterDto
    {
        // لازم Constructor فاضي عشان System.Text.Json يقدر ينشئ الكائن
        public RegisterDto() { }

        // خصائص قابلة للتعيين (settable). خلّي nullable للي ممكن يجي null.
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Email { get; set; }
        public string? Role { get; set; }          // "student" | "instructor" | "admin"
        public string? DisplayName { get; set; }
        public string? FullName { get; set; }
        public string? UniversityId { get; set; }
    }
}
