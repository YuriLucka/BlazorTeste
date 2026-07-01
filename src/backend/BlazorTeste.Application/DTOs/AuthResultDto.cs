namespace BlazorTeste.Application.DTOs;

public class AuthResultDto
{
    public string AccessToken { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiry { get; set; }
}
