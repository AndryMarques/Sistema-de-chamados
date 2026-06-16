import api from './axios'
import type { LoginDTO, TokenResponse, UsuarioCriarDTO, Usuario } from '../types'

export const login = (data: LoginDTO) =>
  api.post<TokenResponse>('/auth/login', data).then((r) => r.data)

export const registrar = (data: UsuarioCriarDTO) =>
  api.post<Usuario>('/auth/registrar', data).then((r) => r.data)
