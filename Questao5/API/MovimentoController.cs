﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MovimentoController> _logger;

        public MovimentoController(IMediator mediator, ILogger<MovimentoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Realiza uma movimentação de conta corrente.
        /// </summary>
        /// <param name="request">Objeto contendo os dados da movimentação.</param>
        /// <returns>Retorna o ID do movimento realizado.</returns>
        /// <response code="200">Movimentação realizada com sucesso.</response>
        /// <response code="400">Se a movimentação for inválida.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MovimentarConta([FromBody] MovimentarContaRequest request)
        {
            _logger.LogInformation("Requisição recebida para movimentar conta: {IdContaCorrente} | Horário: {HoraRecebimento}", request.IdContaCorrente, DateTime.Now);

            var result = (MovimentarContaResponse)await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, tipoErro = result.ErrorType });
            }

            return Ok(new { IdMovimento = result.IdMovimento });
        }
    }
}
