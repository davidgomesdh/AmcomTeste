namespace Questao5.Infrastructure.Data.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<string> ObterPorChave(string chaveIdempotencia);
        Task Salvar(string chaveIdempotencia, string requisicao, string resultado);
    }
}
