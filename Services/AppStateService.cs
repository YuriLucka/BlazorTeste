using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class AppStateService
{
    public Entidade? EntidadeAtiva { get; private set; }
    public bool DarkMode { get; private set; }

    public event Action? OnChange;

    public void SetEntidade(Entidade entidade)
    {
        EntidadeAtiva = entidade;
        OnChange?.Invoke();
    }

    public void ToggleDarkMode()
    {
        DarkMode = !DarkMode;
        OnChange?.Invoke();
    }
}
