# Frontend — Arquitetura e Decisões Técnicas

SPA do Sistema de Controle de Chamados, construída com **React 19**, **TypeScript**, **Tailwind CSS v4** e **Vite**.

> Para instruções de instalação e execução, consulte o [README raiz](../README.md).

---

## Stack

| Tecnologia | Versão | Motivo da escolha |
| --- | --- | --- |
| React 19 | 19 | Biblioteca de UI consolidada; hooks nativos eliminam a necessidade de gerenciadores de estado externos para estado local |
| TypeScript | 6 | Tipos em tempo de compilação: DTOs do backend espelhados em interfaces TS evitam erros de integração |
| Vite | 8 | Build e HMR muito mais rápidos que Webpack/CRA; proxy nativo para o backend durante o desenvolvimento |
| Tailwind CSS v4 | 4 | Utilitário-first elimina CSS customizado; v4 sem arquivo de configuração, integrado diretamente via plugin Vite |
| TanStack Query | 5 | Cache de servidor declarativo: queries com `staleTime`, invalidação automática após mutations, sem gerenciamento manual de `loading/error/data` |
| React Hook Form + Zod | 7 + 4 | RHF evita re-renders por campo; Zod infere os tipos TypeScript do schema, eliminando duplicação entre validação e tipos |
| Axios | 1 | Interceptor global no `axios.ts` injeta o token JWT em todas as requisições automaticamente |
| Sonner | 2 | Toasts com API mínima (`toast.success`, `toast.error`), sem provider obrigatório no componente pai |

---

## Estrutura

```text
src/
├── api/                   # Uma função por operação HTTP, tipada com os DTOs de types/index.ts
│   ├── axios.ts           # Instância única com baseURL e interceptor de Authorization header
│   ├── auth.ts
│   ├── chamados.ts
│   ├── usuarios.ts
│   ├── responsaveis.ts
│   └── acompanhamentos.ts
├── components/
│   ├── Layout/            # Shell: sidebar fixa + área de conteúdo com Outlet
│   └── ui/                # Spinner e ConfirmDialog — componentes sem lógica de negócio
├── context/
│   └── AuthContext.tsx    # Token JWT em memória (não em localStorage): mais seguro contra XSS
├── hooks/
│   └── useToast.ts        # Wrapper tipado do Sonner para padronizar mensagens de erro e sucesso
├── pages/                 # Uma pasta por domínio, um arquivo por página
├── routes/
│   └── index.tsx          # BrowserRouter + ProtectedRoute que redireciona para /login se sem token
└── types/
    └── index.ts           # Interfaces de entidades, DTOs de entrada/saída e enums de status/prioridade
```

---

## Decisões técnicas

### Token JWT em memória

O token é armazenado no estado do `AuthContext` (memória), não em `localStorage` nem `sessionStorage`. Isso elimina o vetor de ataque de XSS que leria o token via `document.cookie` ou `localStorage.getItem`. A desvantagem é que o token se perde ao recarregar a página — o usuário precisa fazer login novamente, o que é aceitável para um sistema interno.

### Camada de API isolada

Cada arquivo em `src/api/` exporta funções puras que retornam `Promise<T>`. As páginas nunca chamam `axios` diretamente — sempre via essas funções. Isso centraliza a tratativa de erros e facilita a substituição do cliente HTTP sem tocar nas páginas.

### TanStack Query como camada de estado do servidor

Não há Redux, Zustand ou Context para dados remotos. O TanStack Query gerencia o ciclo de vida das requisições: cache, revalidação, `isPending`, `isError`. Mutations chamam `queryClient.invalidateQueries` para refrescar dados afetados sem refetch manual.

### Máscara de telefone sem biblioteca

A função `maskTelefone` é uma função pura de string, implementada localmente. Bibliotecas de máscara adicionam peso e frequentemente têm atrito com React Hook Form. A integração com RHF é feita desestruturando `{ ref, onChange, ...rest }` do `register`, sobrescrevendo o `onChange` para aplicar a máscara antes de propagar o evento.

### Captura automática de responsável

Quando um usuário captura um chamado e ainda não tem registro de responsável, a função `obterOuCriarResponsavel` (em `api/responsaveis.ts`) primeiro busca a lista e verifica se já existe um registro para o `usuarioId`. Se não existir, cria automaticamente antes de atribuir o chamado. Essa lógica vive no frontend para não exigir um endpoint novo no backend.

### Validação de formulários em schema

O Zod define o schema de cada formulário. O `zodResolver` do RHF conecta o schema ao formulário sem código imperativo de validação. Os tipos TypeScript dos dados do formulário são inferidos diretamente do schema com `z.infer<typeof schema>`, mantendo um único ponto de verdade.

### Feedback de erros da API

Erros da API são extraídos de forma defensiva: `response.data.message ?? response.data.title ?? Object.values(response.data.errors ?? {}).flat()[0] ?? mensagem_fallback`. Essa cadeia cobre os formatos de erro do ASP.NET Core (validação FluentValidation, `ProblemDetails` e erros customizados).
