# 🚀 SecureAuth API

![.NET](https://img.shields.io/badge/.NET-8-blue)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![CI](https://github.com/sdias-code/portfolio-SecureAuth-local-docker/actions/workflows/ci.yml/badge.svg)


> API de autenticação moderna com JWT + Refresh Token, construída com .NET 8, Clean Architecture e Docker.

---

## 🎯 Objetivo

Demonstrar capacidade técnica no desenvolvimento de APIs modernas e seguras utilizando .NET 8, aplicando boas práticas de arquitetura, autenticação e containerização.

---

## 💡 Sobre o Projeto

SecureAuth API é uma API de autenticação pronta para produção, com foco em segurança, escalabilidade e boas práticas de mercado:

* 🔐 Autenticação JWT + Refresh Token
* 👤 Registro, login e logout
* 🔄 Rotação segura de Refresh Token
* 🧠 CQRS com MediatR
* 🏗 Clean Architecture
* 📊 Logging com Serilog
* 🚦 Rate limiting
* ❤️ Health checks
* 🐳 Docker + PostgreSQL + PGAdmin
* 📄 Swagger (OpenAPI)

---

## 🧠 Arquitetura

O projeto segue **Clean Architecture + CQRS**, utilizando MediatR para desacoplamento:

```text
Controller → IMediator → Command/Query → Handler → Repository
```

### Benefícios

* Baixo acoplamento
* Alta testabilidade
* Separação clara de responsabilidades
* Facilidade de manutenção

---

## 🛠 Tecnologias

* .NET 8 LTS
* C# 12
* Entity Framework Core
* PostgreSQL 16
* Docker + Docker Compose
* Swagger / OpenAPI
* Serilog
* MediatR

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

Crie um arquivo `.env` na raiz:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=secureauth

PGADMIN_EMAIL=demo@example.com
PGADMIN_PASSWORD=secret

JWT__KEY=your_secret_key_here
JWT__ISSUER=SecureAuth
JWT__AUDIENCE=SecureAuthUsers
JWT__EXPIRATION_MINUTES=60

ConnectionStrings__PostgresConnection=Host=postgres;Port=5432;Database=secureauth;Username=postgres;Password=postgres
```

---

## 🐳 Rodando com Docker

### Subir ambiente

```bash
docker-compose up -d --build
```

### Acessos

* API: http://localhost:5000
* Swagger: http://localhost:5000
* PGAdmin: http://localhost:5050

---

## 📌 Endpoints

| Método | Rota               | Descrição           |
| ------ | ------------------ | ------------------- |
| POST   | /api/auth/register | Cadastro            |
| POST   | /api/auth/login    | Login               |
| POST   | /api/auth/refresh  | Refresh Token       |
| POST   | /api/auth/logout   | Logout              |
| GET    | /api/auth/me       | Usuário autenticado |

---

## 🔐 Fluxo de Autenticação

1. Register → cria usuário
2. Login → retorna JWT + RefreshToken
3. Refresh → gera novo JWT
4. Logout → revoga token

✔ Não é necessário enviar `userId` no refresh.

---

## 📝 Observações

* Refresh Token com rotação segura
* Persistência de chaves Data Protection via Docker volume
* Rate limiting em endpoints críticos
* Logging estruturado com Serilog

---

## 📦 Deploy

* Ajustar `.env` para produção
* Subir com Docker em servidor
* Configurar HTTPS

---

## 📚 Licença

MIT License © Silvio Dias Ferreira
