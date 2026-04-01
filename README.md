# SecureAuth API

 - Projeto de autenticação JWT + Refresh Token pronto para produção, com Docker e PostgreSQL.
 - Estrutura Clean Architecture, .NET 8 LTS, pronto para portfólio profissional.

---

## 💡 Sobre o Projeto

SecureAuth API é uma API de autenticação moderna, construída para demonstrar nível pleno/avançado em .NET:

* Autenticação JWT + Refresh Token
* Registro, login, refresh token e logout
* CQRS com MediatR
* Clean Architecture
* Logging com Serilog
* Rate limiting
* Health checks
* Docker-ready (PostgreSQL + PGAdmin)
* Swagger para documentação e testes

Ideal para **freelas backend, portfólio ou base para produto real**.

---

## 🛠 Tecnologias

* .NET 8 LTS
* C# 12
* Entity Framework Core
* PostgreSQL 16
* PGAdmin 9
* Docker + Docker Compose
* Swagger / OpenAPI
* Serilog
* Rate Limiting Middleware

---

## ⚙ Pré-requisitos

* .NET 8 SDK
* Docker
* Docker Compose

---

## 📁 Estrutura do Projeto

```bash
project-root
├── docker-compose.yml
├── Dockerfile
├── .env
├── src
│   ├── SecureAuth.API
│   ├── SecureAuth.Application
│   ├── SecureAuth.Domain
│   └── SecureAuth.Infrastructure
├── tests
│   ├── SecureAuth.UnitTests
│   └── SecureAuth.IntegrationTests
```

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
```

---

## 🐳 Rodando com Docker

### 1️⃣ Subir todos os containers

```bash
docker-compose up -d --build
```

### 2️⃣ Acessos

* API: http://localhost:5000
* Swagger: http://localhost:5000
* PGAdmin: http://localhost:5050

### Containers

| Serviço    | Porta | Observação                   |
| ---------- | ----- | ---------------------------- |
| PostgreSQL | 5432  | Database `secureauth`        |
| API        | 5000  | Swagger em `/` (Development) |
| PGAdmin    | 5050  | Gerenciamento do banco       |

✔ O container da API executa automaticamente as **migrations** no PostgreSQL.

### Parar e remover containers

```bash
docker-compose down -v
```

---

## 📌 Testando a API

Swagger (somente em Development):

```
http://localhost:5000
```

### Endpoints principais

| Método | Rota               | Descrição                     |
| ------ | ------------------ | ----------------------------- |
| POST   | /api/auth/register | Cadastrar usuário             |
| POST   | /api/auth/login    | Login + JWT + Refresh Token   |
| POST   | /api/auth/refresh  | Gerar novo JWT                |
| POST   | /api/auth/logout   | Logout (opcional `logoutAll`) |

---

## 🔐 Fluxo de Autenticação

1. Registrar usuário → `/api/auth/register`
2. Login → `/api/auth/login` → Recebe JWT + Refresh Token
3. Refresh → `/api/auth/refresh` → Novo JWT + Refresh Token
4. Logout → `/api/auth/logout` → Revoga refresh token

✔ Não é necessário enviar `userId` no refresh.

---

## 📝 Observações

* Refresh Token com rotação segura
* Chaves de Data Protection persistidas em volume Docker (`data-protection`)
* Rate limiting ativo em endpoints críticos
* Logging estruturado com Serilog

### Rodando sem Docker

```bash
dotnet ef database update -p src/SecureAuth.API/SecureAuth.API.csproj -s src/SecureAuth.API/SecureAuth.API.csproj
dotnet run --project src/SecureAuth.API/SecureAuth.API.csproj
```

---

## 📦 Deploy / Produção

* Alterar variáveis do `.env`
* Subir com Docker em servidor
* Configurar HTTPS e certificados

---

## 📚 Licença

MIT License © Silvio Dias Ferreira
