using System.Data;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Data.Interfaces;

namespace Questao5.Infrastructure.Data.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnection _dbConnection;

        public MovimentoRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Movimento>> ObterCreditos(string idContaCorrente)
        {
            const string query = "SELECT * FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'C'";
            return await _dbConnection.QueryAsync<Movimento>(query, new { IdContaCorrente = idContaCorrente });
        }

        public async Task<IEnumerable<Movimento>> ObterDebitos(string idContaCorrente)
        {
            const string query = "SELECT * FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'D'";
            return await _dbConnection.QueryAsync<Movimento>(query, new { IdContaCorrente = idContaCorrente });
        }

        public async Task AdicionarMovimento(Movimento movimento)
        {
            const string query = "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) " +
                         "VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";
            await _dbConnection.ExecuteAsync(query, movimento);
        }

        public async Task<Movimento> ObterPorIdMovimento(string idMovimento)
        {
            const string query = "SELECT * FROM movimento WHERE idmovimento = @IdMovimento";
            return await _dbConnection.QueryFirstOrDefaultAsync<Movimento>(query, new { IdMovimento = idMovimento });
        }
    }
}
