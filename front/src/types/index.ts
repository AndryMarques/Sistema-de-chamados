export interface Usuario {
  id: number
  nome: string
  email: string
  telefone: string
  ativo: boolean
  dataCriacao: string
  dataAtualizacao?: string
}

export interface Responsavel {
  id: number
  usuarioId: number
  chamadosEmAberto: number
  dataAssociacao: string
  usuario: Usuario
}

export interface Acompanhamento {
  id: number
  chamadoId: number
  responsavelId: number
  descricao: string
  dataAcompanhamento: string
  responsavel?: Responsavel
}

export interface Chamado {
  id: number
  titulo: string
  descricao: string
  prioridade: number
  status: number
  usuarioId: number
  responsavelId?: number
  dataAbertura: string
  dataResolucao?: string
  dataEncerramento?: string
  dataAtualizacao?: string
  usuario?: Usuario
  responsavel?: Responsavel
  acompanhamentos?: Acompanhamento[]
}

export interface TokenResponse {
  token: string
  usuario: Usuario
  expiresIn: string
}

// DTOs de entrada
export interface LoginDTO {
  email: string
  senha: string
}

export interface UsuarioCriarDTO {
  nome: string
  email: string
  senha: string
  telefone: string
}

export interface UsuarioAtualizarDTO {
  id: number
  nome: string
  email: string
  telefone: string
  ativo: boolean
}

export interface ChamadoCriarDTO {
  titulo: string
  descricao: string
  prioridade: number
  usuarioId: number
}

export interface ChamadoAtualizarDTO {
  id: number
  titulo: string
  descricao: string
  prioridade: number
  status: number
  responsavelId?: number
}

export interface ResponsavelCriarDTO {
  usuarioId: number
}

export interface AcompanhamentoCriarDTO {
  chamadoId: number
  responsavelId: number
  descricao: string
}

// Enums
export const ChamadoStatus: Record<number, string> = {
  1: 'Aberto',
  2: 'Em Andamento',
  3: 'Resolvido',
  4: 'Fechado',
  5: 'Reaberto pelo Cliente',
}

export const ChamadoPrioridade: Record<number, string> = {
  1: 'Baixa',
  2: 'Média',
  3: 'Alta',
}
