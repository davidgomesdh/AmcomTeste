using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Data.Interfaces;
using Xunit;

namespace Questao5.Tests
{
    public class MovimentarContaHandlerTests
    {
        private readonly MovimentarContaHandler _handler;
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentarContaHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
            _idempotenciaRepository = Substitute.For<IIdempotenciaRepository>();
            _handler = new MovimentarContaHandler(_contaCorrenteRepository, _movimentoRepository, _idempotenciaRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnResponse_WhenIdempotenciaExists()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdRequisicao = "123",
                IdContaCorrente = "1",
                Valor = 100,
                TipoMovimento = "C"
            };

            var idempotenciaResponse = new MovimentarContaResponse
            {
                IsSuccess = true,
                IdMovimento = "movimento1",
                Status = "SUCCESS"
            };

            _idempotenciaRepository.ObterPorChave(Arg.Any<string>())
                .Returns(JsonSerializer.Serialize(idempotenciaResponse));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("SUCCESS", result.Status);
            Assert.Equal("movimento1", result.IdMovimento);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContaCorrenteNaoExiste()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdRequisicao = "123",
                IdContaCorrente = "1",
                Valor = 100,
                TipoMovimento = "C"
            };

            _contaCorrenteRepository.ObterPorId(Arg.Any<string>())
                .Returns((ContaCorrente)null); // Conta não existe

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Apenas contas correntes cadastradas podem receber movimentação", result.ErrorMessage);
            Assert.Equal("INVALID_ACCOUNT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContaCorrenteInativa()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdContaCorrente = "123",
                Valor = 100,
                TipoMovimento = "C",
                IdRequisicao = "12345"
            };

            var conta = new ContaCorrente
            {
                IdContaCorrente = "123",
                Ativo = 0 // Conta inativa
            };

            _contaCorrenteRepository.ObterPorId(Arg.Any<string>()).Returns(conta);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Be("Apenas contas correntes ativas podem receber movimentação");
            response.ErrorType.Should().Be("INACTIVE_ACCOUNT");
        }


        [Fact]
        public async Task Handle_ShouldReturnError_WhenValorInvalido()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdRequisicao = "123",
                IdContaCorrente = "1",
                Valor = 0, // Valor inválido
                TipoMovimento = "C"
            };

            var contaCorrente = new ContaCorrente { IdContaCorrente = "1", Ativo = 1 };
            _contaCorrenteRepository.ObterPorId(Arg.Any<string>())
                .Returns(contaCorrente); // Conta ativa

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Apenas valores positivos podem ser recebidos", result.ErrorMessage);
            Assert.Equal("INVALID_VALUE", result.ErrorType);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenTipoMovimentoInvalido()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdRequisicao = "123",
                IdContaCorrente = "1",
                Valor = 100,
                TipoMovimento = "X" // Tipo inválido
            };

            var contaCorrente = new ContaCorrente { IdContaCorrente = "1", Ativo = 1 };
            _contaCorrenteRepository.ObterPorId(Arg.Any<string>())
                .Returns(contaCorrente); // Conta ativa

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Apenas os tipos “débito” ou “crédito” podem ser aceitos", result.ErrorMessage);
            Assert.Equal("INVALID_TYPE", result.ErrorType);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenMovimentacaoValida()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdRequisicao = "123",
                IdContaCorrente = "1",
                Valor = 100,
                TipoMovimento = "C"
            };

            var idempotenciaResponse = new MovimentarContaResponse
            {
                IsSuccess = true,
                IdMovimento = "movimento1",
                Status = "SUCCESS"
            };

            // Serializa a resposta em JSON
            var jsonResponse = JsonSerializer.Serialize(idempotenciaResponse);

            // Configura o mock para retornar a string JSON simulada
            _idempotenciaRepository.ObterPorChave(Arg.Any<string>())
                .Returns(jsonResponse); // Retorna a string JSON simulada

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("SUCCESS", result.Status);
            Assert.Equal("movimento1", result.IdMovimento);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenIdempotenciaIsEmpty()
        {
            // Arrange
            var request = new MovimentarContaRequest
            {
                IdRequisicao = "123",
                IdContaCorrente = "1",
                Valor = 100,
                TipoMovimento = "C"
            };

            // Configura o mock para retornar uma string vazia
            _idempotenciaRepository.ObterPorChave(Arg.Any<string>())
                .Returns(string.Empty); // Retorna uma string vazia (sem dados)

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Apenas contas correntes ativas podem receber movimentação", result.ErrorMessage);
        }
    }
}
