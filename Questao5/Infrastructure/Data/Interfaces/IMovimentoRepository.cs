using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Data.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<IEnumerable<Movimento>> ObterCreditos(string idContaCorrente);
        Task<IEnumerable<Movimento>> ObterDebitos(string idContaCorrente);
        Task AdicionarMovimento(Movimento movimento);
        Task<Movimento> ObterPorIdMovimento(string idMovimento);
    }
}
