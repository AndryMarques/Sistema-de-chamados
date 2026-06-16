# Backend — Arquitetura e Decisões Técnicas

API REST do Sistema de Controle de Chamados, construída em **.NET 9** com **Entity Framework Core 9**, **SQLite** e autenticação **JWT**.

> Para instruções de instalação e execução, consulte o [README raiz](../../README.md).

---

## Stack

| Tecnologia | Versão | Motivo da escolha |
| --- | --- | --- |
| .NET 9 / ASP.NET Core | 9.0 | LTS, performance, suporte nativo a minimal APIs e OpenAPI |
| Entity Framework Core | 9.0 | ORM maduro, migrations declarativas, LINQ type-safe |
| SQLite | embutido | Zero instalação, ideal para desenvolvimento e demonstração sem dependência de servidor |
| JWT Bearer | 9.0 | Autenticação stateless — sem sessão no servidor, escalável horizontalmente |
| BCrypt.Net-Next | — | Hash de senhas resistente a força bruta por ser computacionalmente caro por design |
| FluentValidation | — | Separação de responsabilidade: validação fora dos controladores, regras reutilizáveis e testáveis |
| AutoMapper | — | Elimina mapeamento manual entre entidades e DTOs, reduz código repetitivo |

---

## Arquitetura

O projeto segue uma arquitetura em camadas com separação clara de responsabilidades:

```text
src/
├── API/                   # Camada de apresentação
│   ├── Controllers/       # Recebem requisições HTTP, delegam para serviços, retornam DTOs
│   └── DTOs/              # Objetos de entrada (CriarDTO, AtualizarDTO) e saída (ResponseDTO)
├── Application/           # Camada de aplicação (lógica de negócio)
│   ├── Interfaces/        # Contratos de repositórios (IRepository, IChamadoRepository) e serviços
│   ├── Services/          # Orquestram casos de uso usando repositórios via UnitOfWork
│   └── Validators/        # Regras de validação com FluentValidation por DTO
├── Domain/                # Camada de domínio (núcleo sem dependências externas)
│   ├── Entities/          # Entidades ricas (Chamado, Usuario, Responsavel, Acompanhamento)
│   └── Enums/             # ChamadoStatus, ChamadoPrioridade
└── Infrastructure/        # Camada de infraestrutura (implementações concretas)
    ├── Data/
    │   ├── Context/       # AppDbContext — configuração do EF Core
    │   └── Repositories/  # Repository<T> genérico + especializações (ChamadoRepository)
    └── Configurations/    # Fluent API do EF para mapeamento das entidades
```

---

## Padrões aplicados

### Repository Pattern + Unit of Work

Toda persistência passa por `IUnitOfWork`, que agrupa os repositórios especializados (`IChamadoRepository`, `IResponsavelRepository` etc.). Os serviços nunca tocam o `DbContext` diretamente — isso desacopla a lógica de negócio do ORM e torna os testes unitários possíveis com mocks sem tocar banco de dados.

### Repository genérico com override por entidade

`Repository<T>` implementa as operações CRUD comuns. Para entidades que precisam de `Include` (ex: `Chamado` precisa de `Usuario` e `Responsavel.Usuario`), o `ChamadoRepository` sobrescreve `ObterTodosAsync` e os outros métodos de consulta para adicionar os `ThenInclude` necessários. O método base é declarado `virtual` exatamente para esse propósito.

### Validação na borda da API

Os validadores FluentValidation são executados nos controladores antes de qualquer chamada de serviço. Erros de validação retornam `400 BadRequest` com a lista de mensagens, sem que a lógica de negócio seja acionada.

### Soft-delete implícito por regra de negócio

Não há coluna `deletedAt`. A regra de "não pode deletar" é aplicada no serviço: chamados só podem ser excluídos com status `Fechado`; responsáveis só podem ser removidos se não tiverem chamados em aberto. Isso mantém o histórico íntegro.

---

## Decisões de domínio

### Distribuição de chamados

Ao criar um chamado sem especificar responsável, o sistema busca o responsável com menor `ChamadosEmAberto` (`ObterResponsavelComMenorCargaAsync`). Se um `ResponsavelId` for informado, ele é usado diretamente. O campo `ChamadosEmAberto` é um contador gerenciado explicitamente nos serviços — não uma query derivada — para evitar `COUNT` em toda listagem.

### Status de chamado como fluxo

Os status (`Aberto → EmAndamento → Resolvido → Fechado`) são controlados pelo frontend e validados no serviço de atualização. Transições de data (`DataResolucao`, `DataEncerramento`) são registradas automaticamente na camada de serviço quando o status muda, sem depender do cliente para informar o timestamp.

### Captura de chamado

Qualquer usuário autenticado pode "capturar" um chamado tornando-se responsável. Se o usuário ainda não é um responsável cadastrado, o frontend cria o registro automaticamente antes de atribuir o chamado. A lógica de criação automática de responsável (`obterOuCriarResponsavel`) vive no frontend para evitar um endpoint extra no backend.
