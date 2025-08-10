namespace PetFamily.Auth.Public.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public string Phone { get; set; }
}
