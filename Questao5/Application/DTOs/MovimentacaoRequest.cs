namespace Questao5.Domain.DTOs
{
    public class MovimentacaoRequest
    {
        public Guid ChaveIdempotencia { get; set; }
        public Guid IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }
}
