import api from './axios'
import type { Chamado, ChamadoCriarDTO, ChamadoAtualizarDTO } from '../types'

export const getChamados = () =>
  api.get<Chamado[]>('/chamados').then((r) => r.data)

export const getChamado = (id: number) =>
  api.get<Chamado>(`/chamados/${id}`).then((r) => r.data)

export const getChamadosPorUsuario = (usuarioId: number) =>
  api.get<Chamado[]>(`/chamados/usuario/${usuarioId}`).then((r) => r.data)

export const getChamadosPorResponsavel = (responsavelId: number) =>
  api.get<Chamado[]>(`/chamados/responsavel/${responsavelId}`).then((r) => r.data)

export const criarChamado = (data: ChamadoCriarDTO) =>
  api.post<Chamado>('/chamados', data).then((r) => r.data)

export const atualizarChamado = (id: number, data: ChamadoAtualizarDTO) =>
  api.put<Chamado>(`/chamados/${id}`, data).then((r) => r.data)

export const deletarChamado = (id: number) =>
  api.delete(`/chamados/${id}`)
