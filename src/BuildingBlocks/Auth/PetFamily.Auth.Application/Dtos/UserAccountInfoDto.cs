namespace PetFamily.Auth.Application.Dtos;

public class UserAccountInfoDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string Phone { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? BlockedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Roles { get; set; }
    public List<string> Permissions { get; set; }
}
