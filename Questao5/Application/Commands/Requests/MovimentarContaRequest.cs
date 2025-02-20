using System.ComponentModel.DataAnnotations;
using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
    public class MovimentarContaRequest : IRequest<MovimentarContaResponse>
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string IdRequisicao { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string IdContaCorrente { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(0.01, 999999999999999.00, ErrorMessage = "O campo {0} deve estar entre 0.01 e 999999999999999.00.")]
        public decimal Valor { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string TipoMovimento { get; set; }
    }
}
