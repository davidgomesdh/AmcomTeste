using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Data.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente> ObterPorId(string idContaCorrente);
        Task<ContaCorrente> ObterPorNumero(string numeroContaCorrente);
        Task<IEnumerable<ContaCorrente>> ObterContasAtivas();
        Task<bool> ExisteContaAtiva(string idContaCorrente);
    }
}
