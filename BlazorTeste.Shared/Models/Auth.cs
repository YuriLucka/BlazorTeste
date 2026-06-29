namespace BlazorTeste.Models;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Senha { get; set; } = "";
}

public class LoginResponse
{
    public string AccessToken { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
}
