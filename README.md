### Resposta da QUESTÃO 5

### IMPORTANTE, NOTAS DE ATUALIZAÇÃO DE MELHORIAS
- Adicionado validação no request utilizando Data Annotations
- Adiconado logs utilizando Serilog
- Adicionado authorização e autenticação utilizando JWT Bearer

# Sistema de Consultas de Saldo de Contas Bancárias

## Índice
1. [Visão Geral](#visão-geral)
2. [Tecnologias Utilizadas](#tecnologias-utilizadas)
3. [Instalação](#instalação)
4. [Uso](#uso)
5. [Casos de Uso](#casos-de-uso)
6. [Requisitos](#requisitos-para-rodar-o-sistema)
7. [IMPORTANTE SOBRE AUTENTICAÇÃO](autenticação-e-Autorização-JWT)

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
   git clone https://github.com/davidgomesdh/AmcomTeste.git

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

  ```bash
http://localhost:5000/api/movimento        -- POST
http://localhost:5000/api/consultasaldo/   -- GET
```

## Casos de uso
Você pode colar os seguintes casos de uso no postman para testar:

1. Criar um Novo Movimento (Primeira vez com IdRequisicao)
Este endpoint cria um novo movimento para uma conta corrente utilizando um `IdRequisicao` único.

*Requisição*
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

*Resposta Esperada (Sucesso)*
```json
{
    "IdMovimento": "uuid-do-movimento"
}
```

2. Repetir a Requisição com IdRequisicao Repetido
Esse endpoint valida se a mesma requisição (IdRequisicao) foi realizada anteriormente. Se o IdRequisicao já foi utilizado, a movimentação será rejeitada e exibirá o resultado obtido anteriormente pela IdRequisição correspondente.

*Requisição*
```bash
curl --location 'http://localhost:5000/api/movimento' \
--header 'Content-Type: application/json' \
--data '{
    "IdRequisicao": "123e4567-e89b-12d3-a456-426614174000",
    "IdContaCorrente": "FA99D033-7067-ED11-96C6-7C5DFA4A16C9",
    "TipoMovimento": "C",
    "Valor": 150.00
}'
```
*Resposta Esperada*
```json
{
    "IdMovimento": "uuid-do-movimento"
}
```

3. Criar um Novo Movimento com IdRequisicao Diferente
Este endpoint cria um novo movimento para a conta corrente com um IdRequisicao diferente do anterior.

*Requisição*
```bash
curl --location 'http://localhost:5000/api/movimento' \
--header 'Content-Type: application/json' \
--data '{
    "IdRequisicao": "123e4567-e89b-12d3-a456-426614174000",
    "IdContaCorrente": "FA99D033-7067-ED11-96C6-7C5DFA4A16C9",
    "TipoMovimento": "C",
    "Valor": 150.00
}'
```
*Resposta Esperada (Sucesso)*
```json
{
    "IdMovimento": "uuid-do-movimento"
}
```

4. Testar Conta Inválida
Este endpoint valida o IdContaCorrente para verificar se a conta informada existe. Se a conta não for encontrada ou for inválida, a requisição falha.

*Requisição*
```bash
curl --location 'http://localhost:5000/api/movimento' \
--header 'Content-Type: application/json' \
--data '{
    "IdRequisicao": "456e1234-e21b-45d3-a123-426614174222",
    "IdContaCorrente": "999",
    "TipoMovimento": "C",
    "Valor": 100.00
}'
```
*Resposta Esperada (Erro)*
```json
{
    "message": "Apenas contas correntes cadastradas podem receber movimentação.",
    "tipoErro": "INVALID_ACCOUNT"
}
```

5. Testar Valor Inválido
Este endpoint valida o valor da movimentação. Se o valor informado for inválido (ex: negativo para crédito), a requisição falha.

*Requisição*
```bash
curl --location 'http://localhost:5000/api/movimento' \
--header 'Content-Type: application/json' \
--data '{
    "IdRequisicao": "456e1234-e21b-45d3-a123-426614174222",
    "IdContaCorrente": "999",
    "TipoMovimento": "C",
    "Valor": 100.00
}'
```

*Resposta Esperada (Erro)*
```json
{
    "message": "Apenas valores positivos podem ser recebidos.",
    "tipoErro": "INVALID_VALUE"
}
```

6. Consultar Saldo
Esse endpoint permite consultar o saldo de uma conta corrente específica, fornecendo o IdContaCorrente.

*Requisição*
```bash
curl --location 'http://localhost:5000/api/consultasaldo/FA99D033-7067-ED11-96C6-7C5DFA4A16C9'
```

*Resposta Esperada*
```json
{
    "NumeroContaCorrente": "111",
    "NomeTitular": "João Silva",
    "SaldoAtual": 1500.00,
    "DataHoraConsulta": "2023-02-01T10:00:00"
}
```

# Requisitos para Rodar o Sistema

Antes de iniciar o sistema, certifique-se de que os seguintes requisitos estejam atendidos:

## 1. Ambiente de Desenvolvimento
O sistema foi desenvolvido para rodar em um ambiente local de desenvolvimento. Você precisará dos seguintes itens para executar a aplicação:

- **.NET 6.0 ou superior**: A aplicação é construída utilizando o framework .NET, portanto, você precisará ter o SDK do .NET instalado.
  - Para instalar o .NET, visite [dotnet.microsoft.com](https://dotnet.microsoft.com/download).

- **SQLite**: Utilizamos o SQLite como banco de dados local para armazenar as informações de movimentos e contas correntes.
  - Não é necessário instalar nada manualmente para o SQLite, pois ele é utilizado como banco de dados em arquivo.

- **Editor de Código**: Recomendamos o uso do [Visual Studio Code](https://code.visualstudio.com/) ou [Visual Studio](https://visualstudio.microsoft.com/) para uma melhor experiência de desenvolvimento. 

## 2. Dependências de Pacotes

Os seguintes pacotes NuGet são necessários para rodar o sistema. Eles serão instalados automaticamente quando você restaurar os pacotes:

- **MediatR**: Para facilitar a comunicação entre diferentes camadas da aplicação.
- **Microsoft.Data.Sqlite**: Para interação com o banco de dados SQLite.
- **Swagger**: Para gerar documentação interativa da API (utilizando o pacote `Swashbuckle.AspNetCore`).

## 3. Variáveis de Configuração

A aplicação depende de algumas variáveis de configuração que são necessárias para seu correto funcionamento. As variáveis podem ser definidas no arquivo `appsettings.json` ou diretamente no ambiente de execução.

Exemplo de configuração para o banco de dados:

```json
{
  "DatabaseName": "Data Source=database.sqlite"
}

```
{
  "DatabaseName": "Data Source=database.sqlite"
}

# **Autenticação e Autorização JWT**

Agora que implementamos a autenticação e autorização usando JWT (JSON Web Token), é necessário fornecer um token válido para acessar as rotas protegidas da API. A seguir, explicamos como obter o token e utilizá-lo para fazer requisições autenticadas.

### **1. Obtendo o Token de Autenticação**

Para obter um token JWT, você deve fazer uma requisição POST para o endpoint de login da API com as credenciais de usuário. O endpoint de login pode ser algo como:

## IMPORTANTE - UTILIZE user e password para ser autenticado

*Exemplo de Requisição:*
```http
POST /api/auth/login
curl --location --request POST 'http://localhost:5000/api/auth/login' \
--header 'Content-Type: application/json' \
--data '{
    "username": "user",
    "password": "password"
}'

```

*Resposta Esperada:*
```json
{
    "token": "seuTokenJwtAqui"
}
```

### **1. Utilizando o Token para Acessar Endpoints Protegidos**

Após obter o token JWT, você pode usá-lo para acessar os endpoints protegidos da API. Para isso, basta adicionar o token no cabeçalho da requisição como um Bearer Token.

*Exemplo de Requisição para Acessar um Endpoint Protegido:*
```http
curl --location --request GET 'http://localhost:5000/api/consultasaldo/FA99D033-7067-ED11-96C6-7C5DFA4A16C9' \
--header 'Authorization: Bearer seuTokenJwtAqui'

```
### **3. Erros Comuns**
- [Token Expirado:] Se o token expirou, você receberá o status 401 Unauthorized e precisará se autenticar novamente.
- [Token Inválido:] Caso o token seja inválido ou malformado, o status 400 Bad Request será retornado, indicando que o token não pode ser processado.

### **4. Expiração do Token**
A expiração do token é configurada no backend e pode ser verificada em cada token JWT através da sua payload. Quando o token expirar, você precisará fazer login novamente para obter um novo token.

### **5. Autorização de Acesso**
Alguns endpoints da API podem ter restrições de autorização baseadas em papéis ou permissões (roles). Para acessar essas rotas, você precisa garantir que seu token JWT inclua as permissões necessárias. A autorização pode ser configurada em diferentes níveis de segurança no backend.

Por exemplo, se um endpoint for acessível apenas por usuários com a role "Admin", o sistema verificará a presença dessa role no token antes de permitir o acesso.

