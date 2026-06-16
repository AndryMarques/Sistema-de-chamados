import api from './axios'
import type { Responsavel, ResponsavelCriarDTO } from '../types'

export const getResponsaveis = () =>
  api.get<Responsavel[]>('/responsaveis').then((r) => r.data)

export const getResponsavel = (id: number) =>
  api.get<Responsavel>(`/responsaveis/${id}`).then((r) => r.data)

export const criarResponsavel = (data: ResponsavelCriarDTO) =>
  api.post<Responsavel>('/responsaveis', data).then((r) => r.data)

export const obterOuCriarResponsavel = async (usuarioId: number): Promise<Responsavel> => {
  const lista = await getResponsaveis()
  const existente = lista.find((r) => r.usuarioId === usuarioId)
  if (existente) return existente
  return criarResponsavel({ usuarioId })
}

export const deletarResponsavel = (id: number) =>
  api.delete(`/responsaveis/${id}`)

export const atribuirChamado = (chamadoId: number) =>
  api.post(`/responsaveis/atribuir-chamado/${chamadoId}`).then((r) => r.data)
