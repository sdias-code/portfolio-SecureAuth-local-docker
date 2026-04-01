# SecureAuth API

> 🚀 Projeto de autenticação JWT + Refresh Token pronto para produção, com Docker e PostgreSQL.  
> 🔹 Estrutura Clean Architecture, .NET 8 LTS, pronto para portfólio profissional.

---

## 💡 Sobre o Projeto

SecureAuth API é uma API de autenticação moderna, construída para demonstrar nível pleno/avançado em .NET:

- Autenticação JWT + Refresh Token
- Registro, login, refresh token e logout
- CQRS com MediatR
- Clean Architecture
- Logging com Serilog
- Rate limiting
- Health checks
- Docker-ready (PostgreSQL + PGAdmin)
- Swagger para documentação e testes

Ideal para **freelas backend, portfólio ou base para produto real**.

---

## 🛠 Tecnologias

- .NET 8 LTS
- C# 12
- Entity Framework Core
- PostgreSQL 16
- PGAdmin 9
- Docker + Docker Compose
- Swagger / OpenAPI
- Serilog
- Rate Limiting Middleware

---

## ⚙ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

---

## 📁 Estrutura do Projeto
project-root
├── docker-compose.yml
├── Dockerfile
├── .env
├── src
│ ├── SecureAuth.API
│ ├── SecureAuth.Application
│ ├── SecureAuth.Domain
│ ├── SecureAuth.Infrastructure
├── tests
│ ├── SecureAuth.UnitTests
│ ├── SecureAuth.IntegrationTests


---

## ⚙ Configuração do `.env`

Crie um arquivo `.env` na raiz do projeto com:

```env
# Postgres
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=secureauth

# PGADMIN
PGADMIN_EMAIL=demo@example.com
PGADMIN_PASSWORD=secret

# JWT
JWT__KEY=minhasuperkeyaqui
JWT__ISSUER=SecureAuth
JWT__AUDIENCE=SecureAuthUsers
JWT__EXPIRATION_MINUTES=60

# Connection string
ConnectionStrings__PostgresConnection=Host=postgres;Port=5432;Database=secureauth;Username=postgres;Password=postgres

🐳 Rodando com Docker

1️⃣ Subir todos os containers
docker-compose up -d --build

2️⃣ Acessos
API: http://localhost:5000
Swagger: http://localhost:5000
PGAdmin: http://localhost:5050

Containers:
Serviço	    Porta	Observação
PostgreSQL	5432	Database secureauth
API	        5000	Swagger em / em Dev
PGAdmin	    5050	Gerenciamento do banco
O container da API executa automaticamente as migrations no PostgreSQL.

Para parar e remover containers e volumes:
docker-compose down -v

📌 Testando a API
Swagger (somente em Development):
http://localhost:5000/index.html

Endpoints principais:
Método	Rota	Descrição
POST	/api/auth/register	Cadastrar usuário
POST	/api/auth/login	Login e retorno JWT + Refresh
POST	/api/auth/refresh	Receber novo JWT + Refresh
POST	/api/auth/logout	Logout (opcional logoutAll)

🔐 Fluxo de Autenticação

Registrar usuário → /api/auth/register
Login → /api/auth/login → Recebe JWT + Refresh Token
Refresh → /api/auth/refresh → Novo JWT + Refresh Token
Logout → /api/auth/logout → Revoga refresh token (opcional global)

✅ A API não precisa enviar userId no refresh, basta o refreshToken.

📝 Observações

Chaves de Data Protection persistem em volume Docker (data-protection)
Para rodar local sem Docker, configure .env e execute migrations manualmente:
dotnet ef database update -p src/SecureAuth.API/SecureAuth.API.csproj -s src/SecureAuth.API/SecureAuth.API.csproj
dotnet run --project src/SecureAuth.API/SecureAuth.API.csproj

📦 Deploy/Produção

Alterar variáveis do .env para produção
Subir containers em server Linux/Windows com Docker Compose
Configurar HTTPS e certificados no container de produção

📚 Licença

MIT License © [Silvio Dias Ferreira]