# Sistema de Controle de Chamados

Sistema web para gerenciamento de chamados internos. O backend expõe uma API REST em **.NET 9 + SQLite** e o frontend é uma SPA em **React 19 + TypeScript**.

---

## Pré-requisitos

| Ferramenta | Versão mínima | Download |
| --- | --- | --- |
| .NET SDK | 9.0 | <https://dotnet.microsoft.com/download/dotnet/9> |
| Node.js | 18.0 | <https://nodejs.org> |
| npm | 9.0 | incluído com o Node |
| Git | qualquer | <https://git-scm.com> |

Nenhum servidor de banco de dados precisa ser instalado — o SQLite é embutido.

---

## Instalação

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd Sistema-de-chamados
```

### 2. Configurar e iniciar o backend

```bash
cd back/Sistema-de-chamados
dotnet restore
cp .env.example .env
```

Abra o `.env` e troque apenas o `JwtSettings__Secret` por uma chave de sua escolha (mínimo 32 caracteres):

```env
ConnectionStrings__DefaultConnection=Data Source=sistema-chamados.db
JwtSettings__Secret=troque-por-uma-chave-segura-de-32-chars-ou-mais
JwtSettings__ExpirationMinutes=60
```

Inicie a API:

```bash
dotnet run
```

A API ficará disponível em `http://localhost:5012`. A documentação interativa em `http://localhost:5012/scalar/v1`.

### 3. Configurar e iniciar o frontend

Em outro terminal, a partir da raiz do repositório:

```bash
cd front
npm install
cp .env.example .env
```

O `.env` já vem configurado para apontar para o backend local:

```env
VITE_API_URL=http://localhost:5012
```

Inicie o servidor de desenvolvimento:

```bash
npm run dev
```

A aplicação ficará disponível em `http://localhost:5173`.

---

## Banco de dados

O arquivo `sistema-chamados.db` já está incluído no repositório com dados de exemplo — não é necessário rodar migrações nem popular o banco manualmente.

### Usuário de teste

| Campo | Valor |
| --- | --- |
| E-mail | `teste@gmail.com` |
| Senha | `123456Ab` |

<br>

> Caso queira, para recriar o banco do zero: delete `sistema-chamados.db`, instale `dotnet-ef` com `dotnet tool install --global dotnet-ef` e rode `dotnet ef database update` dentro de `back/Sistema-de-chamados`.

---

## Estrutura do repositório

```text
Sistema-de-chamados/
├── back/
│   └── Sistema-de-chamados/    # API .NET 9
│       ├── src/
│       │   ├── API/            # Controllers e DTOs
│       │   ├── Application/    # Serviços, interfaces e validadores
│       │   ├── Domain/         # Entidades e enums
│       │   └── Infrastructure/ # EF Core, repositórios e contexto
│       ├── sistema-chamados.db # Banco SQLite com dados de exemplo
│       └── README.md           # Arquitetura e decisões técnicas do backend
└── front/
    ├── src/
    │   ├── api/                # Camada HTTP (Axios)
    │   ├── components/         # Componentes reutilizáveis e layout
    │   ├── context/            # AuthContext (JWT em memória)
    │   ├── hooks/              # useToast
    │   ├── pages/              # Chamados, Usuários, Responsáveis
    │   ├── routes/             # Roteamento + ProtectedRoute
    │   └── types/              # Interfaces TypeScript e DTOs
    └── README.md               # Arquitetura e decisões técnicas do frontend
```

---

## Funcionalidades principais

- **Autenticação JWT** — login, cadastro e rotas protegidas
- **Chamados** — criação com atribuição automática (menor carga) ou manual; captura por qualquer usuário; finalização com escolha de status (Resolvido ou Fechado) e justificativa obrigatória
- **Usuários** — listagem, edição com máscara de telefone e feedback de erros da API
- **Responsáveis** — listagem com carga de trabalho em tempo real
- **Acompanhamentos** — histórico de ações em cada chamado
