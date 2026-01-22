# 🧪 SaaS Backend Laboratory & Platform Engineering

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Container-blue?style=flat&logo=docker)
![Postgres](https://img.shields.io/badge/PostgreSQL-Neon.tech-336791?style=flat&logo=postgresql)
![Status](https://img.shields.io/badge/Status-Live-success)

[🇧🇷 Português](#-sobre-o-projeto) | [🇺🇸 English](#-about-the-project)

---

## 🇧🇷 Sobre o Projeto

Este repositório atua como um **Laboratório de Engenharia de Plataforma Backend**. O objetivo principal não é apenas construir uma API funcional, mas implementar padrões arquiteturais robustos focados em **Resiliência, Observabilidade, Segurança e Infraestrutura como Código (IaC)**.

Este projeto serve como base fundamental (template) para aplicações SaaS escaláveis, utilizando .NET 8, PostgreSQL e integração com IA Generativa.

### 🔗 Live Demo / Deploy

A API está rodando em produção no Render (Free Tier).
Você pode testar os endpoints e a documentação via Swagger UI:

👉 **[Acessar Swagger UI (Live)](https://saas-backend-ri81.onrender.com/swagger)**

> ⚠️ **Nota:** Como está hospedado no plano gratuito, a aplicação entra em modo de suspensão por inatividade. A primeira requisição pode levar cerca de **50 segundos** para acordar o servidor (Cold Start).

### 🚀 Funcionalidades de Engenharia (Platform Engineering)

O foco deste laboratório é a implementação de requisitos não-funcionais críticos:

* **Infraestrutura como Código (IaC):**
    * Deploy automatizado via `render.yaml` (Blueprint).
    * Configuração declarativa de serviços e variáveis de ambiente.
* **Observabilidade & Logs:**
    * Logs estruturados (JSON) com **Serilog**.
    * Rastreamento de requisições HTTP e performance de banco de dados em tempo real.
* **Resiliência & Auto-Cura (Self-Healing):**
    * **Deep Health Checks:** Monitoramento ativo da conexão com o banco de dados (PostgreSQL/Neon).
    * Endpoints padronizados (`/health`) com resposta JSON detalhada para orquestradores.
    * Política de "Fail Fast": O sistema reporta erro imediatamente se a infraestrutura crítica falhar.
* **Segurança & Performance:**
    * **Rate Limiting Avançado:** Proteção contra ataques de força bruta, DDoS e loops acidentais.
    * Política inteligente baseada em **IP** com `QueueLimit = 0` (rejeição imediata sem fila de espera).
* **IA Integration:**
    * Implementação do **Semantic Kernel** para orquestração de IA.
    * Integração com Google Gemini Flash para processamento de linguagem natural.

### 🛠️ Tech Stack
* **Core:** .NET 8 (C#)
* **Banco de Dados:** PostgreSQL (via Neon Tech)
* **Containerização:** Docker & Docker Compose
* **Cloud/Deploy:** Render
* **AI:** Microsoft Semantic Kernel + Google Gemini

---

## 🇺🇸 About the Project

This repository serves as a **Backend Platform Engineering Laboratory**. The main goal is not just to build a functional API, but to implement robust architectural patterns focused on **Resilience, Observability, Security, and Infrastructure as Code (IaC)**.

This project acts as a foundational template for scalable SaaS applications, leveraging .NET 8, PostgreSQL, and Generative AI integration.

### 🔗 Live Demo / Deploy

The API is running in production on Render (Free Tier).
You can test endpoints and documentation via Swagger UI:

👉 **[Access Swagger UI (Live)](https://saas-backend-ri81.onrender.com/swagger)**

> ⚠️ **Note:** Hosted on the free tier, the application sleeps after inactivity. The first request may take about **50 seconds** to wake up the server (Cold Start).

### 🚀 Engineering Features (Platform Engineering)

The focus of this lab is the implementation of critical non-functional requirements:

* **Infrastructure as Code (IaC):**
    * Automated deployment via `render.yaml` (Blueprint).
    * Declarative configuration of services and environment variables.
* **Observability & Logging:**
    * Structured logging (JSON) with **Serilog**.
    * Real-time tracing of HTTP requests and database performance.
* **Resilience & Self-Healing:**
    * **Deep Health Checks:** Active monitoring of database connections (PostgreSQL/Neon).
    * Standardized endpoints (`/health`) with detailed JSON responses for orchestrators.
    * "Fail Fast" policy: System immediately reports errors if critical infrastructure fails.
* **Security & Performance:**
    * **Advanced Rate Limiting:** Protection against brute-force attacks, DDoS, and accidental loops.
    * Intelligent **IP-based** policy with `QueueLimit = 0` (immediate rejection without queuing).
* **AI Integration:**
    * **Semantic Kernel** implementation for AI orchestration.
    * Google Gemini Flash integration for natural language processing.

### 🛠️ Tech Stack
* **Core:** .NET 8 (C#)
* **Database:** PostgreSQL (via Neon Tech)
* **Containerization:** Docker & Docker Compose
* **Cloud/Deploy:** Render
* **AI:** Microsoft Semantic Kernel + Google Gemini

---

### 🏃‍♂️ How to Run / Como Rodar

```bash
# Clone the repository
git clone [https://github.com/DeNi-Oliveira/projeto-saas.git](https://github.com/DeNi-Oliveira/projeto-saas.git)

# Enter the backend directory
cd backend/SaasApi

# Run locally (Needs PostgreSQL connection string configured)
dotnet run
