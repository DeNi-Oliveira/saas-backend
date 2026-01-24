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

### 🧪 O que testar agora? (Current Experiments)

O laboratório conta com um **Classificador Financeiro Inteligente**.
No Swagger, tente o endpoint `POST /api/Ai/classify-expenses` com um texto informal:

> **Input (Texto):** "Almocei com o cliente na churrascaria gastando 150 reais e peguei um uber de 30 pra voltar."

> **Output (JSON Gerado):** O sistema identifica categorias, separa valores e gera tags automaticamente.

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
* **AI & Data Engineering:**
    * **Semantic Kernel Orchestration:** Integração nativa com LLMs (Gemini/OpenAI).
    * **Structured Outputs:** Conversão de linguagem natural não-estruturada em objetos JSON tipados e validados (ex: *Expense Classifier*).
    * **System Prompt Engineering:** Uso de instruções de sistema para garantir consistência de dados para o Frontend.

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

### 🧪 What to test now? (Current Experiments)

The lab features an **Intelligent Expense Classifier**.
On Swagger, try the `POST /api/Ai/classify-expenses` endpoint with informal text:

> **Input (Text):** "I had lunch with the client at the steakhouse spending 150 reais and took an uber of 30 to get back."

> **Output (Generated JSON):** The system identifies categories, separates values, and generates tags automatically.

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
* **AI & Data Engineering:**
    * **Semantic Kernel Orchestration:** Native integration with LLMs (Gemini/OpenAI).
    * **Structured Outputs:** Conversion of unstructured natural language into typed and validated JSON objects (e.g., *Expense Classifier*).
    * **System Prompt Engineering:** Use of system instructions to ensure data consistency for the Frontend.

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
git clone [https://github.com/DeNi-Oliveira/saas-backend.git](https://github.com/DeNi-Oliveira/saas-backend.git)

# Enter the backend directory
cd backend/SaasApi

# Run locally (Needs PostgreSQL connection string configured)
dotnet run
