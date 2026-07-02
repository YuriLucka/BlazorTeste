namespace BlazorTeste.Application.DTOs;

public class LoginResultDto
{
    public bool RequiresTwoFactor { get; set; }
    public string? MfaToken { get; set; }
    public AuthResultDto? Auth { get; set; }
}
