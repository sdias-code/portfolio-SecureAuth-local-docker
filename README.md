# 🚀 SecureAuth API

![.NET](https://img.shields.io/badge/.NET-8-blue)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![CI](https://github.com/sdias-code/portfolio-SecureAuth-local-docker/actions/workflows/ci.yml/badge.svg)

## 💼 Disponível para freelas

Este projeto demonstra minha capacidade em construir APIs seguras, escaláveis e prontas para produção.

---

> API de autenticação moderna com JWT + Refresh Token, construída com .NET 8, Clean Architecture e preparada para produção com Docker e CI/CD.

---

## 🎯 Objetivo

Demonstrar domínio em desenvolvimento backend com .NET, aplicando:

- Arquitetura limpa (Clean Architecture)
- Autenticação segura com JWT + Refresh Token
- Testes automatizados com banco real
- Containerização com Docker
- Pipeline CI/CD com GitHub Actions
---

## 💡 Sobre o Projeto

A SecureAuth API é uma solução completa de autenticação, projetada com foco em:

Funcionalidades
- ✔ Registro de usuário
- ✔ Login com geração de JWT
- ✔ Refresh Token seguro (com rotação)
- ✔ Logout individual ou global
- ✔ Endpoint protegido (/me)
- ✔ Rate limiting em endpoints críticos

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
* MediatR (CQRS)
* Serilog (logging estruturado)
* Swagger / OpenAPI
* Rate Limiting (ASP.NET Core)

---

## 🧪 Testes

O projeto possui testes automatizados com foco em cenários reais:

- ✔ Testes de integração com PostgreSQL real
- ✔ Uso de Testcontainers (ambiente isolado)
- ✔ Validação completa do fluxo de autenticação
- ✔ Executados automaticamente via CI/CD

dotnet test

---

## 🔄 CI/CD

Pipeline configurada com GitHub Actions:

- Build da aplicação
- Execução de testes automatizados
- Validação do ambiente Docker
- Pronto para deploy contínuo

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
JWT__EXPIRATIONMINUTES=60

ConnectionStrings__PostgresConnection=Host=postgres;Port=5432;Database=secureauth;Username=postgres;Password=postgres
```

---

## 🐳 Rodando com Docker

### Subir o ambiente completo

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

✔ Não é necessário enviar `userId` no refresh
✔ Refresh Token armazenado de forma segura (hash)

---

## 📝 Observações Técnicas

* Refresh Token com rotação segura
* Validação completa de JWT (issuer, audience, lifetime)
* Logging estruturado com Serilog
* Rate limiting configurado
* Persistência de Data Protection via volume Docker
* Pronto para uso em ambientes reais

---

## 📦 Deploy

Para produção:

- Utilizar variáveis de ambiente seguras (secrets)
- Configurar HTTPS (Nginx ou reverse proxy)
- Ajustar parâmetros de segurança (Rate limit, logs, etc.)

---

## 📚 Licença

MIT License © Silvio Dias Ferreira
