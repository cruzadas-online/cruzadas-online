# Cruzadas.online ✠

Plataforma web de jogos voltada à formação intelectual, cultura cristã, história da Igreja e fé católica.

---

## 1. Visão Geral do MVP v0.1 — Quiz Católico

Este repositório contém o primeiro *vertical slice* completo do **Cruzadas.online**:
- **Catálogo por Grupos Temáticos**: Organização dos jogos em categorias doutrinárias, bíblicas, históricas e litúrgicas:
  - ⛪ **Doutrina e Sacramentos**: *Quiz Católico — Fundamentos da Fé* (Iniciante)
  - 📖 **Sagradas Escrituras**: *Os Evangelhos e o Cânon Bíblico* (Intermediário)
  - 🛡️ **História e Tradição**: *Grandes Concílios e Santos Mártires* (Avançado)
  - 🔥 **Liturgia e Oração**: *O Santo Sacrifício da Missa* (Iniciante)
- **Acesso Híbrido ao Jogo**:
  - **🎲 Partida Rápida (1-clique)**: Sorteia aleatoriamente qualquer quiz publicado e inicia o desafio imediatamente.
  - **Catálogo Curado com Filtros**: Abas/chips para filtrar os quizzes por tema e visualizar o nível de dificuldade.
- **Ciclo completo da partida**:
  - Seleção aleatória de questões por tentativa (`QuizAttempt`).
  - Navegação fluida e acessível questão a questão com suporte a atalhos de teclado (`1-4`, `A-D`, `Enter`).
  - Validação estrita no backend (respostas corretas nunca são expostas ao cliente antes da conclusão).
  - Cálculo de pontuação com percentual e classificação.
  - Revisão detalhada questão a questão com indicação visual de acerto/erro, resposta fornecida, resposta correta, explicação teológica e referência oficial.
  - Opção de reiniciar a partida (*Jogar Novamente*) ou retornar ao catálogo.
  - Jogador anônimo sem barreiras de autenticação nesta versão.

---

## 2. Arquitetura da Solução

A solução segue princípios de **Clean Architecture** e **Domain-Driven Design (DDD) Pragmático**, priorizando alta coesão, baixo acoplamento e testabilidade sem camadas artificiais ou over-engineering:

```text
cruzadas-online/
├── apps/
│   ├── api/                                # Backend .NET 10
│   │   ├── src/
│   │   │   ├── Cruzadas.Domain/            # Entidades, Enums, Regras de Domínio e Exceções
│   │   │   ├── Cruzadas.Application/       # DTOs, Interfaces de Serviço e QuizService
│   │   │   ├── Cruzadas.Infrastructure/    # EF Core DbContext, Mapeamentos, Seeder e Migrations
│   │   │   └── Cruzadas.Api/               # ASP.NET Core 10 Minimal APIs, Scalar OpenAPI, ProblemDetails
│   │   └── tests/
│   │       ├── Cruzadas.Domain.Tests/      # Testes unitários de regras de negócio
│   │       ├── Cruzadas.Application.Tests/ # Testes do serviço da aplicação com DB em memória
│   │       └── Cruzadas.Api.IntegrationTests/ # Testes de integração E2E com WebApplicationFactory
│   │
│   └── web/                                # Frontend React 19 + TypeScript + Vite 8
│       ├── src/
│       │   ├── api/                        # Cliente HTTP tipado com tratamento de erros
│       │   ├── components/                 # Componentes acessíveis (Header, Catalog, QuizPlay, Result, Footer)
│       │   ├── test/                       # Testes de fluxo e integração com Vitest e Testing Library
│       │   ├── types/                      # Interfaces TypeScript sincronizadas com os DTOs
│       │   └── index.css                   # Design tokens: paleta solene (Azul Catedral, Vinho Cardinalício, Dourado)
│
├── infra/                                  # Dockerfiles e configurações de proxy
│   ├── Dockerfile.api                      # Multi-stage build .NET 10 (SDK -> ASP.NET Runtime)
│   ├── Dockerfile.web                      # Multi-stage build (Node 24 -> Nginx Alpine)
│   └── nginx.conf                          # Reverse proxy para rotas /api/ e SPA fallback
│
├── docker-compose.yml                      # Orquestração local isolada (nome: cruzadas-online)
├── .env.example                            # Variáveis de ambiente e mapeamento de portas seguras
└── project-handoff.md                      # Handoff operacional, aprendizados e decisões arquiteturais
```

---

## 3. Stack Tecnológica

| Componente | Tecnologia | Versão Estável / Baseline |
| :--- | :--- | :--- |
| **Backend Runtime** | .NET LTS | 10.0 |
| **Web Framework** | ASP.NET Core | 10.0 |
| **ORM & Database Provider** | EF Core / Npgsql | 10.0 |
| **Banco de Dados** | PostgreSQL | 18 / alpine |
| **Documentação API** | Scalar OpenAPI | v1 |
| **Frontend UI** | React | 19.3 |
| **Linguagem Frontend** | TypeScript | 5.9 / ES2023 |
| **Build Tool & Bundler** | Vite | 8.3 |
| **Testes Backend** | xUnit, FluentAssertions | net10.0 |
| **Testes Frontend** | Vitest, React Testing Library | v5 / v16 |
| **Containerização** | Docker Compose | Compose Spec v2 |

---

## 4. Portas e Isolamento do Ambiente

Para garantir coexistência harmoniosa com outros containers ou serviços rodando no host, as seguintes portas não-conflitantes foram alocadas:

| Serviço | Porta Host Padrão | Variável de Ambiente |
| :--- | :--- | :--- |
| **Frontend Web** | `http://localhost:5175` | `CRUZADAS_WEB_PORT` |
| **Backend API** | `http://localhost:5185` | `CRUZADAS_API_PORT` |
| **PostgreSQL** | `localhost:55432` | `CRUZADAS_DB_PORT` |

---

## 5. Como Executar

### Opção A: Execução via Docker Compose (Recomendada)

1. Clone o repositório e configure as variáveis de ambiente:
   ```bash
   cp .env.example .env
   ```

2. Suba o ambiente isolado do projeto:
   ```bash
   docker compose up --build -d
   ```

3. Acesse no navegador:
   - **Frontend**: [http://localhost:5175](http://localhost:5175)
   - **API Health**: [http://localhost:5185/health](http://localhost:5185/health)
   - **API OpenAPI / Scalar**: [http://localhost:5185/scalar/v1](http://localhost:5185/scalar/v1)

4. Para parar os containers do Cruzadas.online sem afetar outros projetos:
   ```bash
   docker compose down
   ```

---

### Opção B: Execução Local para Desenvolvimento (Host)

#### 1. Banco de Dados PostgreSQL
Suba apenas o PostgreSQL do projeto ou utilize sua instância local:
```bash
docker compose up cruzadas-db -d
```

#### 2. Backend API (.NET 10)
```bash
dotnet run --project apps/api/src/Cruzadas.Api/Cruzadas.Api.csproj
```
A API será iniciada em `http://localhost:5185` (ou na porta configurada no `launchSettings.json`). Em modo `Development`, as migrations do EF Core e o seed de dados são aplicados automaticamente na inicialização.

#### 3. Frontend (React + Vite)
```bash
cd apps/web
npm install
npm run dev
```
O frontend estará acessível em `http://localhost:5175`.

---

## 6. Testes Automatizados

### Testes do Backend (.NET)
Executa todos os testes de Domínio, Aplicação e Integração HTTP E2E:
```bash
dotnet test apps/api/Cruzadas.slnx
```
- **Total**: 31 testes executados (31 aprovados, 0 falhas).

### Testes do Frontend (Vitest)
Executa a validação de componentes, renderização e fluxo completo do usuário:
```bash
cd apps/web
npm test -- --run
```
- **Total**: 100% de sucesso (2 testes de integração de fluxo).

### Validação de Lint e Build do Frontend
```bash
cd apps/web
npm run lint
npm run build
```

---

## 7. Endpoints da API REST

| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `GET` | `/health` | Health check da API e conectividade do banco |
| `GET` | `/api/v1/games` | Lista de jogos disponíveis no catálogo (suporta `?group={slug}`) |
| `GET` | `/api/v1/games/groups` | Lista os grupos e categorias temáticas de quiz com seus jogos associados |
| `GET` | `/api/v1/quizzes/{slug}` | Detalhes e metadados de um quiz específico |
| `POST` | `/api/v1/quizzes/{slug}/attempts` | Inicia uma nova partida com questões sorteadas |
| `POST` | `/api/v1/quizzes/random/attempts` | Inicia uma partida rápida com sorteio aleatório entre todos os quizzes |
| `POST` | `/api/v1/quizzes/{slug}/attempts/{attemptId}/complete` | Envia as respostas da partida, calcula nota e retorna revisão detalhada |
| `GET` | `/scalar/v1` | Documentação interativa da API via Scalar OpenAPI |

---

## 8. Licença
Projeto Cruzadas.online — Todos os direitos reservados.
