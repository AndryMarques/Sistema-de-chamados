# Sistema de Controle de Chamados - Backend

## 📋 Descrição
Sistema de controle de chamados internos desenvolvido com **.NET 9**, **Entity Framework Core**, **Repository Pattern** e **JWT Authentication**.

## 🏗️ Arquitetura
- **API REST** com padrão MVC
- **Entity Framework Core** para acesso a dados
- **Repository Pattern** para abstração de dados
- **Service Layer** para lógica de negócio
- **JWT Authentication** para autenticação de usuários
- **FluentValidation** para validação de entrada
- **AutoMapper** para mapeamento de entidades
- **SQL Server** como banco de dados

## 🚀 Pré-requisitos
- .NET 9 SDK instalado
- SQL Server (localdb ou instalado)
- Visual Studio 2026 Community ou Superior (opcional)

## 📦 Instalação

### 1. Restaurar dependências
```bash
dotnet restore
```

### 2. Configurar a connection string
Editar `appsettings.json` com sua connection string do SQL Server:
```json
"ConnectionStrings": {
	"DefaultConnection": "Server=seu-servidor;Database=SistemaChamamdos;Trusted_Connection=true;"
}
```

### 3. Configurar JWT Secret
Editar `appsettings.json` com uma chave secreta segura:
```json
"JwtSettings": {
	"Secret": "sua-chave-secreta-muito-longa-e-segura",
	"ExpirationMinutes": 60
}
```

### 4. Criar e aplicar migrações
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Executar a aplicação
```bash
dotnet run
```

A API estará disponível em: `https://localhost:5001`

## 📁 Estrutura do Projeto

```
src/
├── API/
│   ├── Controllers/          # Controllers da API
│   └── DTOs/                 # Data Transfer Objects
├── Application/
│   ├── Services/             # Lógica de negócio
│   ├── Interfaces/           # Interfaces dos serviços
│   └── Validators/           # Validadores FluentValidation
├── Infrastructure/
│   ├── Data/
│   │   ├── Context/          # DbContext
│   │   └── Repositories/     # Repositórios
│   └── Configurations/       # Configurações
└── Domain/
	├── Entities/             # Entidades de domínio
	└── Enums/                # Enumerações
```

## 🔐 Autenticação JWT
Todas as requisições à API devem incluir o token JWT no header:
```
Authorization: Bearer {seu-token-aqui}
```

## 📝 Próximos Passos
- [ ] Step 2: Modelagem de Dados (concluído com Step 1)
- [ ] Step 3: Contexto do Entity Framework (concluído com Step 1)
- [ ] Step 4: Camada de Repositório
- [ ] Step 5: Camada de Serviços
- [ ] Step 6: Controllers/Endpoints
- [ ] Step 7: Testes Unitários
- [ ] Step 8: Documentação

## 📚 Dependências Principais
- Microsoft.EntityFrameworkCore 9.0.0
- Microsoft.EntityFrameworkCore.SqlServer 9.0.0
- AutoMapper 12.0.1
- FluentValidation 11.10.0
- System.IdentityModel.Tokens.Jwt 8.1.0
- Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0

## 🤝 Contribuindo
Siga os padrões de código estabelecidos e sempre adicione testes para novas funcionalidades.

## 📧 Contato
Para dúvidas ou sugestões, abra uma issue no repositório.
