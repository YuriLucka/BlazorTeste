using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class AppStateService
{
    public Entidade? EntidadeAtiva { get; private set; }
    public bool DarkMode { get; private set; }

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

    private void NotifyStateChanged() => OnChange?.Invoke();
}
