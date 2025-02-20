using System.Text.Json;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Data.Interfaces;

namespace Questao5.Application.Handlers
{
    public class MovimentarContaHandler : IRequestHandler<MovimentarContaRequest, MovimentarContaResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentarContaHandler(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository, IIdempotenciaRepository idempotenciaRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<MovimentarContaResponse> Handle(MovimentarContaRequest request, CancellationToken cancellationToken)
        {
            var resultadoExistente = await _idempotenciaRepository.ObterPorChave(request.IdRequisicao);
            if (resultadoExistente != null)
            {
                return JsonSerializer.Deserialize<MovimentarContaResponse>(resultadoExistente);
            }

            var conta = await _contaCorrenteRepository.ObterPorId(request.IdContaCorrente);

            if (conta == null)
                return new MovimentarContaResponse { IsSuccess = false, ErrorMessage = "Apenas contas correntes cadastradas podem receber movimentação", ErrorType = "INVALID_ACCOUNT" };

            if (conta.Ativo == 0)
                return new MovimentarContaResponse { IsSuccess = false, ErrorMessage = "Apenas contas correntes ativas podem receber movimentação", ErrorType = "INACTIVE_ACCOUNT" };

            if (request.Valor <= 0)
                return new MovimentarContaResponse { IsSuccess = false, ErrorMessage = "Apenas valores positivos podem ser recebidos", ErrorType = "INVALID_VALUE" };

            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                return new MovimentarContaResponse { IsSuccess = false, ErrorMessage = "Apenas os tipos “débito” ou “crédito” podem ser aceitos", ErrorType = "INVALID_TYPE" };

            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.Now,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };

            try
            {
                await _movimentoRepository.AdicionarMovimento(movimento);

                var response = new MovimentarContaResponse
                {
                    IsSuccess = true,
                    IdMovimento = movimento.IdMovimento,
                    Status = "SUCCESS"
                };

                await _idempotenciaRepository.Salvar(Guid.NewGuid().ToString(), request.IdRequisicao, JsonSerializer.Serialize(response));

                return response;
            }
            catch (Exception ex)
            {
                return new MovimentarContaResponse
                {
                    IsSuccess = false,
                    Status = "ERROR",
                    ErrorMessage = $"Erro ao processar a movimentação: {ex.Message}"
                };
            }
        }
    }
}
