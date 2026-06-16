# Sistema de Controle de Chamados — Backend

API REST para gerenciamento de chamados internos, desenvolvida com **.NET 9**, **Entity Framework Core**, **SQL Server** e autenticação via **JWT**.

---

## Pré-requisitos

Antes de começar, certifique-se de ter instalado na sua máquina:

| Ferramenta | Versão mínima | Download |
|---|---|---|
| .NET SDK | 9.0 | https://dotnet.microsoft.com/download/dotnet/9 |
| SQL Server | 2019 ou LocalDB | https://www.microsoft.com/sql-server/sql-server-downloads |
| Git | qualquer | https://git-scm.com |

> **LocalDB** já vem instalado com o Visual Studio. Para verificar se está disponível, rode `sqllocaldb info` no terminal.

---

## Instalação passo a passo

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd Sistema-de-chamados/back/Sistema-de-chamados
```

---

### 2. Restaurar as dependências

```bash
dotnet restore
```

Esse comando baixa automaticamente todos os pacotes NuGet:

- `Microsoft.EntityFrameworkCore.SqlServer` — ORM e driver do SQL Server
- `Microsoft.AspNetCore.Authentication.JwtBearer` — autenticação JWT
- `AutoMapper` — mapeamento entre entidades e DTOs
- `FluentValidation` — validação de entrada
- `BCrypt.Net-Next` — hash de senhas

---

### 3. Configurar as variáveis de ambiente

Copie o arquivo de exemplo e preencha com os valores do seu ambiente:

```bash
cp .env.example .env
```

Abra o `.env` e ajuste a connection string:

```env
# LocalDB (padrão — não precisa instalar nada além do SDK)
ConnectionStrings__DefaultConnection=Server=(localdb)\mssqllocaldb;Database=SistemaChamamdos;Trusted_Connection=true;

# SQL Server local com autenticação Windows
# ConnectionStrings__DefaultConnection=Server=.\SQLEXPRESS;Database=SistemaChamamdos;Trusted_Connection=true;TrustServerCertificate=true;

# SQL Server com usuário e senha
# ConnectionStrings__DefaultConnection=Server=localhost;Database=SistemaChamamdos;User Id=sa;Password=sua_senha;TrustServerCertificate=true;
```

> O arquivo `.env` é ignorado pelo Git. **Nunca commite credenciais reais no repositório.**

---

### 4. Configurar o JWT Secret

No mesmo `.env`, defina uma chave secreta com **no mínimo 32 caracteres**:

```env
JwtSettings__Secret=troque-por-uma-chave-muito-segura-de-32-chars-ou-mais
JwtSettings__ExpirationMinutes=60
```

> O duplo underscore (`__`) é a convenção do ASP.NET Core para mapear variáveis de ambiente para seções de configuração: `JwtSettings__Secret` → `JwtSettings:Secret`.

---

### 5. Instalar a ferramenta do Entity Framework

Se ainda não tiver o `dotnet-ef` instalado globalmente:

```bash
dotnet tool install --global dotnet-ef
```

Para verificar se já está instalado:

```bash
dotnet ef --version
```

---

### 6. Criar e aplicar as migrações do banco de dados

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

- O primeiro comando gera os arquivos de migração com base nas entidades do projeto.
- O segundo cria o banco de dados e todas as tabelas.

> Se o banco `SistemaChamamdos` não existir, o EF o cria automaticamente.

---

### 7. Executar a aplicação

```bash
dotnet run
```

A API ficará disponível em:

```text
http://localhost:5012
https://localhost:7122
```

A documentação interativa (Scalar UI) estará em:

```text
http://localhost:5012/scalar/v1
```

O JSON bruto do OpenAPI estará em:

```text
http://localhost:5012/openapi/v1.json
```

---

## Executar os testes

O projeto de testes está em `../Sistema-de-chamados.Tests`. Para rodá-los:

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

Todas as configurações ficam no arquivo `.env` (criado a partir de `.env.example`):

| Chave | Descrição | Padrão |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | String de conexão com o SQL Server | LocalDB |
| `JwtSettings:Secret` | Chave secreta para assinar os tokens JWT | *(trocar antes de usar)* |
| `JwtSettings:ExpirationMinutes` | Tempo de expiração do token em minutos | `60` |

---

## Decisões técnicas

- **Repository Pattern + Unit of Work** — desacopla a lógica de negócio do ORM, facilitando testes unitários com mocks.
- **JWT stateless** — sem sessão no servidor; o token carrega as informações necessárias.
- **BCrypt** para hash de senhas — resistente a ataques de força bruta por ser computacionalmente caro por design.
- **Distribuição automática de chamados** — ao criar um chamado sem responsável definido, o sistema atribui automaticamente ao responsável com menor número de chamados em aberto.
- **Soft-delete implícito** — chamados só podem ser deletados quando estão com status `Fechado`; usuários com chamados em aberto também não podem ser removidos.
