# Sistema de Controle de Chamados — Backend

API REST para gerenciamento de chamados internos, desenvolvida com **.NET 9**, **Entity Framework Core 9**, **SQLite** e autenticação via **JWT**.

> O banco de dados SQLite já está incluído no repositório com dados de exemplo — não é necessário rodar migrações nem popular o banco manualmente.

---

## Pré-requisitos

| Ferramenta | Versão mínima | Download |
|---|---|---|
| .NET SDK | 9.0 | https://dotnet.microsoft.com/download/dotnet/9 |
| Git | qualquer | https://git-scm.com |

Nenhum servidor de banco de dados precisa ser instalado.

---

## Instalação passo a passo

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd Sistema-de-chamados/back/Sistema-de-chamados
```

### 2. Restaurar as dependências

```bash
dotnet restore
```

Pacotes instalados automaticamente:

- `Microsoft.EntityFrameworkCore.Sqlite` — ORM e driver SQLite
- `Microsoft.AspNetCore.Authentication.JwtBearer` — autenticação JWT
- `AutoMapper` — mapeamento entre entidades e DTOs
- `FluentValidation` — validação de entrada
- `BCrypt.Net-Next` — hash de senhas

### 3. Configurar as variáveis de ambiente

```bash
cp .env.example .env
```

O `.env.example` já contém a connection string correta para SQLite. Troque apenas o `JwtSettings__Secret`:

```env
ConnectionStrings__DefaultConnection=Data Source=sistema-chamados.db
JwtSettings__Secret=troque-por-uma-chave-segura-de-32-chars-ou-mais
JwtSettings__ExpirationMinutes=60
```

> O duplo underscore (`__`) mapeia variáveis de ambiente para seções de configuração do ASP.NET Core: `JwtSettings__Secret` → `JwtSettings:Secret`.

### 4. Executar a aplicação

```bash
dotnet run
```

A API ficará disponível em:

```text
http://localhost:5012
https://localhost:7122
```

A documentação interativa (Scalar UI):

```text
http://localhost:5012/scalar/v1
```

---

## Banco de dados incluído

O arquivo `sistema-chamados.db` está versionado com dados de exemplo prontos para uso:

| Entidade | Dados de exemplo |
| --- | --- |
| Usuários | 1 usuário administrador cadastrado |
| Responsáveis | 1 responsável vinculado ao usuário |
| Chamados | Chamados de teste em diferentes status |

> Para recriar o banco do zero: delete `sistema-chamados.db`, instale `dotnet-ef` (`dotnet tool install --global dotnet-ef`) e rode `dotnet ef database update`.

---

## Executar os testes

```bash
cd ../Sistema-de-chamados.Tests
dotnet test
```

---

## Estrutura do projeto

```text
src/
├── API/
│   ├── Controllers/       # Endpoints REST (Auth, Chamados, Usuários, Responsáveis, Acompanhamentos)
│   └── DTOs/              # Objetos de entrada e saída da API
├── Application/
│   ├── Interfaces/        # Contratos de repositórios e serviços
│   ├── Services/          # Lógica de negócio
│   └── Validators/        # Validadores FluentValidation
├── Domain/
│   ├── Entities/          # Entidades de domínio
│   └── Enums/             # Enumerações (Status, Prioridade)
└── Infrastructure/
    ├── Data/
    │   ├── Context/       # DbContext (AppDbContext)
    │   └── Repositories/  # Implementações dos repositórios
    └── Configurations/    # Configurações do EF (Fluent API)
```

---

## Variáveis de configuração

| Chave | Descrição | Padrão |
| --- | --- | --- |
| `ConnectionStrings__DefaultConnection` | Caminho do arquivo SQLite | `Data Source=sistema-chamados.db` |
| `JwtSettings__Secret` | Chave secreta para assinar tokens JWT | *(trocar antes de usar)* |
| `JwtSettings__ExpirationMinutes` | Expiração do token em minutos | `60` |

---

## Decisões técnicas

- **SQLite** — banco embutido, zero instalação, ideal para desenvolvimento e demonstração.
- **Repository Pattern + Unit of Work** — desacopla a lógica de negócio do ORM, facilitando testes unitários com mocks.
- **JWT stateless** — sem sessão no servidor; o token carrega as informações necessárias.
- **BCrypt** para hash de senhas — resistente a ataques de força bruta por ser computacionalmente caro por design.
- **Distribuição automática de chamados** — ao criar um chamado, o sistema pode atribuir automaticamente ao responsável com menor carga ou permitir seleção manual.
- **Soft-delete implícito** — chamados só podem ser deletados quando estão com status `Fechado`; responsáveis com chamados em aberto também não podem ser removidos.
