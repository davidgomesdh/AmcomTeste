using MediatR;
using Questao5.Application.Queries;
using Questao5.Application.Queries.Requests;
using Questao5.Infrastructure.Data.Interfaces;

namespace Questao5.Application.Handlers
{
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoRequest, ConsultarSaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ConsultarSaldoHandler(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<ConsultarSaldoResponse> Handle(ConsultarSaldoRequest request, CancellationToken cancellationToken)
        {
            var conta = await _contaCorrenteRepository.ObterPorId(request.IdContaCorrente);

            if (conta == null)
                return new ConsultarSaldoResponse { IsSuccess = false, ErrorMessage = "INVALID_ACCOUNT", ErrorType = "INVALID_ACCOUNT" };

            if (conta.Ativo == 0)
                return new ConsultarSaldoResponse { IsSuccess = false, ErrorMessage = "INACTIVE_ACCOUNT", ErrorType = "INACTIVE_ACCOUNT" };

            var creditos = await _movimentoRepository.ObterCreditos(request.IdContaCorrente);
            var debitos = await _movimentoRepository.ObterDebitos(request.IdContaCorrente);

            var saldoAtual = creditos.Sum(c => c.Valor) - debitos.Sum(d => d.Valor);

            return new ConsultarSaldoResponse
            {
                IsSuccess = true,
                NumeroContaCorrente = conta.Numero.ToString(),
                NomeTitular = conta.Nome,
                SaldoAtual = saldoAtual,
                DataHoraConsulta = DateTime.Now
            };
        }
    }
}
