using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Data.Interfaces;
using Xunit;

namespace Questao5.Tests
{
    public class ConsultarSaldoHandlerTests
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly ConsultarSaldoHandler _handler;

        public ConsultarSaldoHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
            _handler = new ConsultarSaldoHandler(_contaCorrenteRepository, _movimentoRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContaNaoEncontrada()
        {
            // Arrange
            var request = new ConsultarSaldoRequest
            {
                IdContaCorrente = "12345"
            };

            _contaCorrenteRepository.ObterPorId(Arg.Any<string>()).Returns((ContaCorrente)null);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Be("INVALID_ACCOUNT");
            response.ErrorType.Should().Be("INVALID_ACCOUNT");
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContaInativa()
        {
            // Arrange
            var request = new ConsultarSaldoRequest
            {
                IdContaCorrente = "12345"
            };

            var contaInativa = new ContaCorrente
            {
                IdContaCorrente = "12345",
                Ativo = 0, // Conta inativa
                Numero = 123456789,
                Nome = "João Silva"
            };

            _contaCorrenteRepository.ObterPorId(Arg.Any<string>()).Returns(contaInativa);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Be("INACTIVE_ACCOUNT");
            response.ErrorType.Should().Be("INACTIVE_ACCOUNT");
        }

        [Fact]
        public async Task Handle_ShouldReturnSaldo_WhenMovimentacaoValida()
        {
            // Arrange
            var request = new ConsultarSaldoRequest
            {
                IdContaCorrente = "12345"
            };

            var conta = new ContaCorrente
            {
                IdContaCorrente = "12345",
                Ativo = 1,
                Numero = 123456789,
                Nome = "João Silva"
            };

            var creditos = new[]
            {
                new Movimento { Valor = 500, TipoMovimento = "C" },
                new Movimento { Valor = 200, TipoMovimento = "C" }
            };

            var debitos = new[]
            {
                new Movimento { Valor = 300, TipoMovimento = "D" },
                new Movimento { Valor = 100, TipoMovimento = "D" }
            };

            _contaCorrenteRepository.ObterPorId(Arg.Any<string>()).Returns(conta);
            _movimentoRepository.ObterCreditos(Arg.Any<string>()).Returns(creditos.AsEnumerable());
            _movimentoRepository.ObterDebitos(Arg.Any<string>()).Returns(debitos.AsEnumerable());

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.SaldoAtual.Should().Be(300); // 500 + 200 - 300 - 100
            response.NumeroContaCorrente.Should().Be("123456789");
            response.NomeTitular.Should().Be("João Silva");
        }

        [Fact]
        public async Task Handle_ShouldReturnSaldo_WhenMovimentacaoComValoresNegativos()
        {
            // Arrange
            var request = new ConsultarSaldoRequest
            {
                IdContaCorrente = "12345"
            };

            var conta = new ContaCorrente
            {
                IdContaCorrente = "12345",
                Ativo = 1,
                Numero = 123456789,
                Nome = "João Silva"
            };

            var creditos = new[]
            {
                new Movimento { Valor = 500, TipoMovimento = "C" },
                new Movimento { Valor = 200, TipoMovimento = "C" }
            };

            var debitos = new[]
            {
                new Movimento { Valor = 600, TipoMovimento = "D" },
                new Movimento { Valor = 200, TipoMovimento = "D" }
            };

            _contaCorrenteRepository.ObterPorId(Arg.Any<string>()).Returns(conta);
            _movimentoRepository.ObterCreditos(Arg.Any<string>()).Returns(creditos.AsEnumerable());
            _movimentoRepository.ObterDebitos(Arg.Any<string>()).Returns(debitos.AsEnumerable());

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.SaldoAtual.Should().Be(-100); // 500 + 200 - 600 - 200
            response.NumeroContaCorrente.Should().Be("123456789");
            response.NomeTitular.Should().Be("João Silva");
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenErroInesperadoNoRepositorio()
        {
            // Arrange
            var request = new ConsultarSaldoRequest
            {
                IdContaCorrente = "12345"
            };

            // Configura o mock para lançar uma exceção
            _contaCorrenteRepository.ObterPorId(Arg.Any<string>())
                                     .Throws(new Exception("Erro inesperado"));

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Contain("Erro inesperado");
            response.ErrorType.Should().Be("ERROR");
        }
    }
}
