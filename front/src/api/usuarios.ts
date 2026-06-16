import api from './axios'
import type { Usuario, UsuarioAtualizarDTO } from '../types'

export const getUsuarios = () =>
  api.get<Usuario[]>('/usuarios').then((r) => r.data)

export const getUsuario = (id: number) =>
  api.get<Usuario>(`/usuarios/${id}`).then((r) => r.data)

export const atualizarUsuario = (id: number, data: UsuarioAtualizarDTO) =>
  api.put<Usuario>(`/usuarios/${id}`, data).then((r) => r.data)

export const deletarUsuario = (id: number) =>
  api.delete(`/usuarios/${id}`)
