# Sistema de Consultas de Saldo de Contas Bancárias

## Índice
1. [Visão Geral](#visão-geral)
2. [Tecnologias Utilizadas](#tecnologias-utilizadas)
3. [Instalação](#instalação)
4. [Uso](#uso)
5. [API Endpoints](#api-endpoints)
6. [Exemplo de Requisição/Resposta](#exemplo-de-requisiçãoresposta)
7. [Contribuindo](#contribuindo)
8. [Licença](#licença)

## Visão Geral
  Este é um serviço de API RESTful que permite consultar saldos de contas bancárias e realizar movimentações financeiras em um banco utilizando SQLite. A API oferece funcionalidades como consulta de saldo, criação de movimentos de débito e crédito, e controle de idempotência.

## Tecnologias Utilizadas
- **ASP.NET Core**: Framework principal utilizado para construir a API.
- **SQLite**: Banco de dados utilizado para armazenar informações de contas e transações.
- **MediatR**: Biblioteca para tratamento de requests e handlers de forma desacoplada.
- **Swagger**: Ferramenta para gerar e documentar a API automaticamente.
- **NSubstitute**: Framework de mock utilizado para testes unitários.
- **xUnit**: Framework de testes utilizado para testes automatizados.

## Instalação

1. Clone este repositório:
   ```bash
   git clone https://github.com/usuario/projeto.git

2. Entre no diretório do projeto:
  cd projeto

3. Restaure as dependências do projeto:
  dotnet restore

4. Crie o banco de dados e configure o ambiente (se necessário):
  O projeto utiliza um banco de dados SQLite que será criado automaticamente durante a execução.

5. Execute a aplicação:
  dotnet run

6. Acesse a API em http://localhost:5000 ou conforme configurado no seu arquivo appsettings.json.

## Uso
  Após executar a aplicação, você pode realizar requisições para os seguintes endpoints:

## Casos de uso
Você pode colar os seguintes casos de uso no postman para testar:

Criar um Novo Movimento (Primeira vez com IdRequisicao)
Este endpoint cria um novo movimento para uma conta corrente utilizando um `IdRequisicao` único.

### Requisição
```bash
curl --location 'http://localhost:5000/api/movimento' \
--header 'Content-Type: application/json' \
--data '{
    "IdRequisicao": "123e4567-e89b-12d3-a456-426614174001",
    "IdContaCorrente": "FA99D033-7067-ED11-96C6-7C5DFA4A16C9",
    "TipoMovimento": "C",
    "Valor": 150.00
}'

```

*Descrição dos Campos*
IdRequisicao: Identificador único para a requisição (tipo UUID).
IdContaCorrente: ID da conta corrente que sofrerá a movimentação.
TipoMovimento: Tipo de movimento ("C" para crédito ou "D" para débito).
Valor: Valor da movimentação.

## Resposta Esperada
```json
{
    "Mensagem": "Movimento realizado com sucesso.",
    "IdMovimento": "uuid-do-movimento"
}
```

