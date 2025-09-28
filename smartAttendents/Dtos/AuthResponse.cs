namespace smartAttendents.Dtos
{
    public record AuthResponse(int Id, string Username, string? DisplayName, string Role, string Token);
}
