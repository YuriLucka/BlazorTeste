namespace BlazorTeste.Models;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Senha { get; set; } = "";
}

public class LoginResponse
{
    public bool RequiresTwoFactor { get; set; }
    public string? MfaToken { get; set; }
    public string AccessToken { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
}

public class TwoFactorVerifyRequest
{
    public string MfaToken { get; set; } = "";
    public string Code { get; set; } = "";
}

public class TwoFactorCodeRequest
{
    public string Code { get; set; } = "";
}

public class TwoFactorSetupResponse
{
    public string SharedKey { get; set; } = "";
    public string QrCodePngBase64 { get; set; } = "";
}
