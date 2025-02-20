using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Queries;
using Questao5.Application.Queries.Requests;

namespace Questao5.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultaSaldoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MovimentoController> _logger;

        public ConsultaSaldoController(IMediator mediator, ILogger<MovimentoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Consultar o saldo de uma conta corrente.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da conta corrente.</param>
        /// <returns>Retorna o saldo da conta corrente.</returns>
        /// <response code="200">Retorna os detalhes da conta com o saldo atual.</response>
        /// <response code="400">Se a conta estiver inativa ou não existir.</response>
        [HttpGet("{idContaCorrente}")]
        [Authorize]
        public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
        {
            _logger.LogInformation("Consulta de saldo solicitada para a conta: {IdContaCorrente} | Horário: {HoraRecebimento}", idContaCorrente, DateTime.Now);

            ConsultarSaldoResponse result = (ConsultarSaldoResponse)await _mediator.Send(new ConsultarSaldoRequest { IdContaCorrente = idContaCorrente });

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, tipoErro = result.ErrorType });
            }

            return Ok(new
            {
                NumeroContaCorrente = result.NumeroContaCorrente,
                NomeTitular = result.NomeTitular,
                SaldoAtual = result.SaldoAtual,
                DataHoraConsulta = result.DataHoraConsulta
            });
        }
    }
}
