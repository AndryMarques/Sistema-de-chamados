# Sistema de Controle de Chamados — Frontend

Interface web para o sistema de gerenciamento de chamados internos, desenvolvida com **React 19**, **TypeScript**, **Tailwind CSS v4** e **Vite**.

> O backend deve estar rodando antes de iniciar o frontend. Veja as instruções em [`back/Sistema-de-chamados/README.md`](../back/Sistema-de-chamados/README.md).

---

## Pré-requisitos

| Ferramenta | Versão mínima | Download |
| --- | --- | --- |
| Node.js | 18.0 | <https://nodejs.org> |
| npm | 9.0 | incluído com o Node |
| Git | qualquer | <https://git-scm.com> |

---

## Instalação passo a passo

### 1. Entrar na pasta do frontend

```bash
cd Sistema-de-chamados/front
```

### 2. Instalar as dependências

```bash
npm install
```

Pacotes principais instalados automaticamente:

- `react` + `react-dom` — biblioteca de UI
- `react-router-dom` — roteamento client-side
- `@tanstack/react-query` — gerenciamento de estado do servidor
- `react-hook-form` + `zod` — formulários com validação em schema
- `axios` — cliente HTTP
- `sonner` — notificações (toasts)
- `tailwindcss` — estilização utilitária

### 3. Configurar as variáveis de ambiente

```bash
cp .env.example .env
```

O arquivo `.env` deve conter a URL da API:

```env
VITE_API_URL=http://localhost:5012
```

Ajuste a porta caso o backend esteja em outro endereço.

### 4. Iniciar o servidor de desenvolvimento

```bash
npm run dev
```

A aplicação ficará disponível em:

```text
http://localhost:5173
```

---

## Scripts disponíveis

| Comando | Descrição |
| --- | --- |
| `npm run dev` | Inicia o servidor de desenvolvimento com hot reload |
| `npm run build` | Gera o build de produção em `dist/` |
| `npm run preview` | Pré-visualiza o build de produção localmente |
| `npm run lint` | Verifica o código com ESLint |

---

## Estrutura do projeto

```text
src/
├── api/                   # Funções de requisição HTTP por recurso
│   ├── axios.ts           # Instância configurada do Axios (interceptors, baseURL)
│   ├── auth.ts
│   ├── chamados.ts
│   ├── usuarios.ts
│   ├── responsaveis.ts
│   └── acompanhamentos.ts
├── components/
│   ├── Layout/            # Shell da aplicação (sidebar, header)
│   └── ui/                # Componentes reutilizáveis (Spinner, ConfirmDialog)
├── context/
│   └── AuthContext.tsx    # Estado global de autenticação + token JWT
├── hooks/
│   └── useToast.ts        # Wrapper do Sonner para toasts padronizados
├── pages/
│   ├── Login.tsx
│   ├── Register.tsx
│   ├── Dashboard.tsx
│   ├── Chamados/
│   │   ├── ChamadosList.tsx
│   │   ├── ChamadoForm.tsx
│   │   └── ChamadoDetail.tsx
│   ├── Usuarios/
│   │   └── UsuariosList.tsx
│   └── Responsaveis/
│       └── ResponsaveisList.tsx
├── routes/
│   └── index.tsx          # Definição de rotas + ProtectedRoute
└── types/
    └── index.ts           # Interfaces TypeScript e DTOs
```

---

## Funcionalidades

### Autenticação

- Login com e-mail e senha
- Cadastro de novo usuário com máscara de telefone
- Token JWT armazenado em memória (via Context)
- Redirecionamento automático para login em rotas protegidas

### Chamados

- Listagem com filtros por status
- Criação com atribuição **automática** (menor carga) ou **manual** (escolha de responsável)
- Detalhe com histórico de acompanhamentos
- **Capturar**: qualquer usuário pode assumir um chamado e se tornar responsável
- **Finalizar**: responsável escolhe entre *Resolvido* ou *Fechado* e registra justificativa
- Exclusão apenas de chamados com status Fechado

### Usuários

- Listagem, edição e exclusão
- Campo de telefone com máscara `(XX) XXXXX-XXXX`
- Feedback de erros da API (e-mail duplicado, formato inválido etc.)

### Responsáveis

- Listagem com carga de trabalho atual
- Associação e remoção de responsáveis

---

## Variáveis de ambiente

| Variável | Descrição | Padrão |
| --- | --- | --- |
| `VITE_API_URL` | URL base da API backend | `http://localhost:5012` |
