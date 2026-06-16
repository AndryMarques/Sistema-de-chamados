# 📚 ENDPOINTS API - Sistema de Controle de Chamados

## 🔓 Endpoints Públicos (Sem JWT)

### 🔐 Autenticação

| Método | Endpoint | Descrição | Request | Response |
|--------|----------|-----------|---------|----------|
| `POST` | `/api/auth/login` | Fazer login | `{ email, senha }` | `{ token, usuario, expiresIn }` |
| `POST` | `/api/auth/registrar` | Criar novo usuário | `{ nome, email, senha, telefone }` | `{ id, nome, email, telefone, ativo, dataCriacao }` |

---

## 🔒 Endpoints Protegidos (Requer JWT)

### 👥 Usuários

| Método | Endpoint | Descrição | Request | Response |
|--------|----------|-----------|---------|----------|
| `GET` | `/api/usuarios` | Listar todos os usuários | - | `IEnumerable<UsuarioResponseDTO>` |
| `GET` | `/api/usuarios/{id}` | Obter usuário por ID | - | `UsuarioResponseDTO` |
| `PUT` | `/api/usuarios/{id}` | Atualizar usuário | `{ id, nome, email, telefone, ativo }` | `UsuarioResponseDTO` |
| `DELETE` | `/api/usuarios/{id}` | Deletar usuário | - | 204 No Content |

### 🎟️ Chamados

| Método | Endpoint | Descrição | Request | Response |
|--------|----------|-----------|---------|----------|
| `POST` | `/api/chamados` | Criar novo chamado ⭐ | `{ titulo, descricao, prioridade, usuarioId }` | `ChamadoResponseDTO` |
| `GET` | `/api/chamados` | Listar todos os chamados | - | `IEnumerable<ChamadoResponseDTO>` |
| `GET` | `/api/chamados/{id}` | Obter chamado com acompanhamentos | - | `ChamadoResponseDTO` |
| `GET` | `/api/chamados/usuario/{usuarioId}` | Obter chamados do usuário | - | `IEnumerable<ChamadoResponseDTO>` |
| `GET` | `/api/chamados/responsavel/{responsavelId}` | Obter chamados do responsável | - | `IEnumerable<ChamadoResponseDTO>` |
| `PUT` | `/api/chamados/{id}` | Atualizar chamado | `{ id, titulo, descricao, prioridade, status, responsavelId }` | `ChamadoResponseDTO` |
| `DELETE` | `/api/chamados/{id}` | Deletar chamado (apenas fechados) | - | 204 No Content |

### 👨‍💼 Responsáveis

| Método | Endpoint | Descrição | Request | Response |
|--------|----------|-----------|---------|----------|
| `POST` | `/api/responsaveis` | Criar novo responsável | `{ usuarioId }` | `ResponsavelResponseDTO` |
| `GET` | `/api/responsaveis` | Listar responsáveis (por carga de trabalho) | - | `IEnumerable<ResponsavelResponseDTO>` |
| `GET` | `/api/responsaveis/{id}` | Obter responsável com chamados | - | `ResponsavelResponseDTO` |
| `DELETE` | `/api/responsaveis/{id}` | Deletar responsável | - | 204 No Content |
| `POST` | `/api/responsaveis/atribuir-chamado/{chamadoId}` | Redistribuir chamado automaticamente | - | `{ message }` |

### 📝 Acompanhamentos

| Método | Endpoint | Descrição | Request | Response |
|--------|----------|-----------|---------|----------|
| `POST` | `/api/acompanhamentos` | Criar acompanhamento | `{ chamadoId, responsavelId, descricao }` | `AcompanhamentoResponseDTO` |
| `GET` | `/api/acompanhamentos/{id}` | Obter acompanhamento por ID | - | `AcompanhamentoResponseDTO` |
| `GET` | `/api/acompanhamentos/chamado/{chamadoId}` | Obter acompanhamentos do chamado | - | `IEnumerable<AcompanhamentoResponseDTO>` |
| `GET` | `/api/acompanhamentos/responsavel/{responsavelId}` | Obter acompanhamentos do responsável | - | `IEnumerable<AcompanhamentoResponseDTO>` |
| `DELETE` | `/api/acompanhamentos/{id}` | Deletar acompanhamento | - | 204 No Content |

---

## 📊 Resumo Geral

| Recurso | GET | POST | PUT | DELETE | Total |
|---------|-----|------|-----|--------|-------|
| Auth | - | 2 | - | - | 2 |
| Usuários | 2 | - | 1 | 1 | 4 |
| Chamados | 4 | 1 | 1 | 1 | 7 |
| Responsáveis | 2 | 2 | - | 1 | 5 |
| Acompanhamentos | 3 | 1 | - | 1 | 5 |
| **TOTAL** | **11** | **6** | **2** | **4** | **28** |

---

## 🎯 Casos de Uso

### 📌 Cenário 1: Novo Usuário Abre um Chamado

```
1. POST /api/auth/registrar
   ↓ Novo usuário criado
2. POST /api/auth/login
   ↓ Recebe token JWT
3. POST /api/chamados
   ↓ Chamado criado e distribuído AUTOMATICAMENTE
	  ao responsável com MENOR CARGA
4. GET /api/chamados/{id}
   ↓ Visualiza seu chamado com responsável atribuído
```

### 📌 Cenário 2: Responsável Atualiza Chamado

```
1. GET /api/responsaveis
   ↓ Visualiza todos ordenados por carga
2. GET /api/chamados/responsavel/{id}
   ↓ Vê seus chamados abertos
3. POST /api/acompanhamentos
   ↓ Adiciona acompanhamento ao chamado
4. PUT /api/chamados/{id}
   ↓ Muda status para "Resolvido"
   ↓ DataResolucao preenchida AUTOMATICAMENTE
5. GET /api/acompanhamentos/chamado/{id}
   ↓ Visualiza histórico de acompanhamentos
```

### 📌 Cenário 3: Gerenciamento de Responsáveis

```
1. GET /api/usuarios
   ↓ Visualiza todos os usuários
2. POST /api/responsaveis
   ↓ Promove usuário para responsável
3. GET /api/responsaveis
   ↓ Verifica carga de trabalho
   ↓ Responsável com menor carga recebe próximos chamados
4. DELETE /api/responsaveis/{id}
   ↓ Só permitido se sem chamados abertos
```

---

## 🔑 Autenticação JWT

### Header Obrigatório (em endpoints protegidos)

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Estrutura do Token

```json
{
  "iss": "SistemaChamamdos",
  "aud": "SistemaChamamdosApp",
  "sub": "1",
  "name": "João Silva",
  "email": "joao@example.com",
  "exp": 1718446800,
  "iat": 1718443200
}
```

---

## ✅ Status HTTP Retornados

| Código | Significado | Cenário |
|--------|-------------|---------|
| `200` | OK | Requisição bem-sucedida (GET, PUT) |
| `201` | Created | Recurso criado com sucesso (POST) |
| `204` | No Content | Recurso deletado com sucesso (DELETE) |
| `400` | Bad Request | Validação falhou / Dados inválidos |
| `401` | Unauthorized | Token inválido, expirado ou ausente |
| `404` | Not Found | Recurso não encontrado |
| `409` | Conflict | Conflito de negócio (ex: email duplicado) |
| `500` | Server Error | Erro geral do servidor |

---

## 🚀 Exemplo Completo: Fluxo de Login e Criar Chamado

### 1️⃣ Login

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "joao@example.com",
	"senha": "Senha123"
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
	"id": 1,
	"nome": "João Silva",
	"email": "joao@example.com",
	"telefone": "11999999999",
	"ativo": true,
	"dataCriacao": "2025-06-15T10:00:00Z"
  },
  "expiresIn": "2025-06-15T11:00:00Z"
}
```

### 2️⃣ Criar Chamado (com token)

```bash
curl -X POST http://localhost:5001/api/chamados \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Erro ao fazer login",
	"descricao": "Não consigo acessar a aplicação",
	"prioridade": 3,
	"usuarioId": 1
  }'
```

**Response:**
```json
{
  "id": 5,
  "titulo": "Erro ao fazer login",
  "descricao": "Não consigo acessar a aplicação",
  "prioridade": 3,
  "status": 1,
  "usuarioId": 1,
  "responsavelId": 2,
  "dataAbertura": "2025-06-15T10:35:00Z",
  "dataResolucao": null,
  "dataEncerramento": null,
  "dataAtualizacao": null,
  "usuario": {
	"id": 1,
	"nome": "João Silva",
	"email": "joao@example.com",
	"telefone": "11999999999",
	"ativo": true
  },
  "responsavel": {
	"id": 2,
	"usuarioId": 2,
	"chamadosEmAberto": 3,
	"dataAssociacao": "2025-06-10T08:00:00Z",
	"usuario": {
	  "id": 2,
	  "nome": "Maria Santos",
	  "email": "maria@example.com"
	}
  },
  "acompanhamentos": []
}
```

### 3️⃣ Adicionar Acompanhamento

```bash
curl -X POST http://localhost:5001/api/acompanhamentos \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
	"chamadoId": 5,
	"responsavelId": 2,
	"descricao": "Identificado o problema nas permissões do usuário"
  }'
```

**Response:**
```json
{
  "id": 1,
  "chamadoId": 5,
  "responsavelId": 2,
  "descricao": "Identificado o problema nas permissões do usuário",
  "dataAcompanhamento": "2025-06-15T10:40:00Z",
  "responsavel": {...}
}
```

### 4️⃣ Atualizar Chamado para Resolvido

```bash
curl -X PUT http://localhost:5001/api/chamados/5 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
	"id": 5,
	"titulo": "Erro ao fazer login",
	"descricao": "Resolvido ajustando as permissões",
	"prioridade": 3,
	"status": 3,
	"responsavelId": 2
  }'
```

**Response:**
```json
{
  "id": 5,
  ...
  "status": 3,
  "dataResolucao": "2025-06-15T10:50:00Z",
  "dataAtualizacao": "2025-06-15T10:50:00Z",
  ...
}
```

### 5️⃣ Visualizar Acompanhamentos

```bash
curl -X GET http://localhost:5001/api/acompanhamentos/chamado/5 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Response:**
```json
[
  {
	"id": 1,
	"chamadoId": 5,
	"responsavelId": 2,
	"descricao": "Identificado o problema nas permissões do usuário",
	"dataAcompanhamento": "2025-06-15T10:40:00Z",
	"responsavel": {...}
  }
]
```

---

## 💡 Dicas de Uso

1. **Token Expira**: Configure `JwtSettings:ExpirationMinutes` em `appsettings.json`
2. **Sempre use Authorization Header**: Em endpoints protegidos, incluir `Authorization: Bearer {token}`
3. **Distribuição Automática**: Ao criar chamado, é atribuído ao responsável com menor `ChamadosEmAberto`
4. **Deletar Chamado**: Apenas chamados com `status = 4 (Fechado)` podem ser deletados
5. **Deletar Responsável**: Apenas responsáveis com `ChamadosEmAberto = 0` podem ser deletados

---

## 📖 Documentação Swagger

Acesse a documentação interativa em:
```
http://localhost:5001/openapi/v1.json
```

Ou use o Swagger UI em:
```
http://localhost:5001/swagger
```

---

**Total de 28 Endpoints Implementados** ✅
