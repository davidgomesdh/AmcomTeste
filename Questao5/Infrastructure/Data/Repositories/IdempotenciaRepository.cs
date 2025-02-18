using Dapper;
using Questao5.Infrastructure.Data.Interfaces;
using System.Data;

namespace Questao5.Infrastructure.Data.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDbConnection _dbConnection;

        public IdempotenciaRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> ObterPorChave(string requisicao)
        {
            const string query = "SELECT resultado FROM idempotencia WHERE requisicao = @Requisicao";
            return await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { Requisicao = requisicao });
        }

        public async Task Salvar(string chaveIdempotencia, string requisicao, string resultado)
        {
            const string query = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
            await _dbConnection.ExecuteAsync(query, new { ChaveIdempotencia = chaveIdempotencia, Requisicao = requisicao, Resultado = resultado });
        }
    }
}
