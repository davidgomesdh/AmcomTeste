using MediatR;

namespace Questao5.Application.Queries.Requests
{
    public class ConsultarSaldoRequest : IRequest<ConsultarSaldoResponse>
    {
        public string IdContaCorrente { get; set; }
    }
}
