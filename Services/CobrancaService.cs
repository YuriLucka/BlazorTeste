using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class CobrancaService
{
    private readonly List<Cobranca> _cobrancas;

    public CobrancaService(ContribuinteService contribuinteService)
    {
        _cobrancas = GenerateCobrancas(contribuinteService.GetAll());
    }

    private static List<Cobranca> GenerateCobrancas(List<Contribuinte> contribuintes)
    {
        var rng = new Random(42);
        var tipos = Enum.GetValues<TipoCobranca>();
        var result = new List<Cobranca>();

        for (int i = 0; i < 80; i++)
        {
            var contribuinte = contribuintes[rng.Next(contribuintes.Count)];
            var tipo = tipos[rng.Next(tipos.Length)];
            var vencimento = DateTime.Today.AddDays(rng.Next(-90, 60));
            var valor = Math.Round((decimal)(rng.NextDouble() * 2000 + 100), 2);
            var pago = rng.Next(3) == 0;
            var vencido = !pago && vencimento < DateTime.Today;

            result.Add(new Cobranca
            {
                Id = i + 1,
                EntidadeId = contribuinte.EntidadeId,
                ContribuinteId = contribuinte.Id,
                RazaoSocialContribuinte = contribuinte.RazaoSocial,
                Tipo = tipo,
                Valor = valor,
                Multa = vencido ? Math.Round(valor * 0.02m, 2) : 0,
                Juros = vencido ? Math.Round(valor * 0.01m * Math.Max(0m, (decimal)(DateTime.Today - vencimento).TotalDays / 30), 2) : 0,
                Vencimento = vencimento,
                DataPagamento = pago ? vencimento.AddDays(rng.Next(-5, 5)) : null,
                Status = pago ? StatusCobranca.Pago : vencido ? StatusCobranca.Vencido : StatusCobranca.Pendente,
                LinhaDigitavel = $"34191.79001 01043.510047 91020.15000{rng.Next(1, 9)} {rng.Next(1, 9)} {rng.Next(10000, 99999)}0000000",
                AnoReferencia = DateTime.Today.Year
            });
        }

        return result;
    }

    public List<Cobranca> GetAll() => _cobrancas;
    public List<Cobranca> GetByStatus(StatusCobranca status) => _cobrancas.Where(c => c.Status == status).ToList();
    public List<Cobranca> GetByContribuinte(int contribuinteId) => _cobrancas.Where(c => c.ContribuinteId == contribuinteId).ToList();
}
