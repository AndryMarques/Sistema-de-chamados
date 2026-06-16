import api from './axios'
import type { Acompanhamento, AcompanhamentoCriarDTO } from '../types'

export const getAcompanhamento = (id: number) =>
  api.get<Acompanhamento>(`/acompanhamentos/${id}`).then((r) => r.data)

export const getAcompanhamentosPorChamado = (chamadoId: number) =>
  api.get<Acompanhamento[]>(`/acompanhamentos/chamado/${chamadoId}`).then((r) => r.data)

export const getAcompanhamentosPorResponsavel = (responsavelId: number) =>
  api.get<Acompanhamento[]>(`/acompanhamentos/responsavel/${responsavelId}`).then((r) => r.data)

export const criarAcompanhamento = (data: AcompanhamentoCriarDTO) =>
  api.post<Acompanhamento>('/acompanhamentos', data).then((r) => r.data)

export const deletarAcompanhamento = (id: number) =>
  api.delete(`/acompanhamentos/${id}`)
