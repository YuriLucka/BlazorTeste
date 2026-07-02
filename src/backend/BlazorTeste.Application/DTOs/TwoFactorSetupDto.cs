namespace BlazorTeste.Application.DTOs;

public class TwoFactorSetupDto
{
    public string SharedKey { get; set; } = "";
    public string QrCodePngBase64 { get; set; } = "";
}
