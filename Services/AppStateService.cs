using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class AppStateService
{
    public Entidade? EntidadeAtiva { get; private set; }
    public bool DarkMode { get; private set; }
    public Usuario? UsuarioLogado { get; private set; }

    public event Action? OnChange;

    public void SetEntidade(Entidade? entidade)
    {
        EntidadeAtiva = entidade;
        NotifyStateChanged();
    }

    public void ToggleDarkMode()
    {
        DarkMode = !DarkMode;
        NotifyStateChanged();
    }

    public void Login(Usuario usuario)
    {
        UsuarioLogado = usuario;
        NotifyStateChanged();
    }

    public void Logout()
    {
        UsuarioLogado = null;
        EntidadeAtiva = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
