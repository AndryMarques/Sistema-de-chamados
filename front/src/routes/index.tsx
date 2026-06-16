import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import Layout from '../components/Layout/Layout'
import Login from '../pages/Login'
import Register from '../pages/Register'
import Dashboard from '../pages/Dashboard'
import ChamadosList from '../pages/Chamados/ChamadosList'
import ChamadoDetail from '../pages/Chamados/ChamadoDetail'
import ChamadoForm from '../pages/Chamados/ChamadoForm'
import UsuariosList from '../pages/Usuarios/UsuariosList'
import ResponsaveisList from '../pages/Responsaveis/ResponsaveisList'

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth()
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />
}

export default function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <Layout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Dashboard />} />
          <Route path="chamados" element={<ChamadosList />} />
          <Route path="chamados/novo" element={<ChamadoForm />} />
          <Route path="chamados/:id" element={<ChamadoDetail />} />
          <Route path="usuarios" element={<UsuariosList />} />
          <Route path="responsaveis" element={<ResponsaveisList />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
