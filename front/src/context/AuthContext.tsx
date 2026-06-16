import { createContext, useContext, useState, useCallback, type ReactNode } from 'react'
import type { Usuario, LoginDTO, UsuarioCriarDTO } from '../types'
import { login as apiLogin, registrar as apiRegistrar } from '../api/auth'

interface AuthContextValue {
  usuario: Usuario | null
  token: string | null
  isAuthenticated: boolean
  login: (data: LoginDTO) => Promise<void>
  registrar: (data: UsuarioCriarDTO) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

const getStoredUsuario = (): Usuario | null => {
  try {
    const raw = localStorage.getItem('usuario')
    return raw ? JSON.parse(raw) : null
  } catch {
    return null
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(getStoredUsuario)
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'))

  const login = useCallback(async (data: LoginDTO) => {
    const response = await apiLogin(data)
    localStorage.setItem('token', response.token)
    localStorage.setItem('usuario', JSON.stringify(response.usuario))
    setToken(response.token)
    setUsuario(response.usuario)
  }, [])

  const registrar = useCallback(async (data: UsuarioCriarDTO) => {
    await apiRegistrar(data)
  }, [])

  const logout = useCallback(() => {
    localStorage.removeItem('token')
    localStorage.removeItem('usuario')
    setToken(null)
    setUsuario(null)
  }, [])

  return (
    <AuthContext.Provider value={{ usuario, token, isAuthenticated: !!token, login, registrar, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
