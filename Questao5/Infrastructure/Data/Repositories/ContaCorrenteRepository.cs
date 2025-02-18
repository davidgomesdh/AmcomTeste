using System.Data;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Data.Interfaces;

namespace Questao5.Infrastructure.Data.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDbConnection _dbConnection;

        public ContaCorrenteRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ContaCorrente> ObterPorId(string idContaCorrente)
        {
            const string query = "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente";
            return await _dbConnection.QuerySingleOrDefaultAsync<ContaCorrente>(query, new { IdContaCorrente = idContaCorrente });
        }

        public async Task<ContaCorrente> ObterPorNumero(string numeroContaCorrente)
        {
            const string query = "SELECT * FROM contacorrente WHERE numero = @Numero";
            return await _dbConnection.QuerySingleOrDefaultAsync<ContaCorrente>(query, new { Numero = numeroContaCorrente });
        }

        public async Task<IEnumerable<ContaCorrente>> ObterContasAtivas()
        {
            const string query = "SELECT * FROM contacorrente WHERE ativo = 1";
            return await _dbConnection.QueryAsync<ContaCorrente>(query);
        }

        public async Task<bool> ExisteContaAtiva(string idContaCorrente)
        {
            const string query = "SELECT COUNT(1) FROM contacorrente WHERE idcontacorrente = @IdContaCorrente AND ativo = 1";
            return await _dbConnection.ExecuteScalarAsync<bool>(query, new { IdContaCorrente = idContaCorrente });
        }
    }
}
