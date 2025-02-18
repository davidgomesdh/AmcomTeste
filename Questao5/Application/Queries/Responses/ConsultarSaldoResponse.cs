namespace Questao5.Application.Queries
{
    public class ConsultarSaldoResponse : ResponseBase
    {
        public string NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public decimal SaldoAtual { get; set; }
        public DateTime DataHoraConsulta { get; set; }
    }
}
