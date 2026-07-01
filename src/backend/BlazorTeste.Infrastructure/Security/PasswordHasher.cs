using BlazorTeste.Application.Security;

namespace BlazorTeste.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => PasswordHelper.Hash(password);
    public bool Verify(string password, string storedHash) => PasswordHelper.Verify(password, storedHash);
}
