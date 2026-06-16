import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getUsuarios, atualizarUsuario, deletarUsuario } from '../../api/usuarios'
import { useToast } from '../../hooks/useToast'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import Spinner from '../../components/ui/Spinner'
import type { Usuario } from '../../types'
import type { AxiosError } from 'axios'

function maskTelefone(value: string): string {
  const digits = value.replace(/\D/g, '').slice(0, 11)
  if (digits.length <= 2) return `(${digits}`
  if (digits.length <= 6) return `(${digits.slice(0, 2)}) ${digits.slice(2)}`
  if (digits.length <= 10) return `(${digits.slice(0, 2)}) ${digits.slice(2, 6)}-${digits.slice(6)}`
  return `(${digits.slice(0, 2)}) ${digits.slice(2, 7)}-${digits.slice(7)}`
}

export default function UsuariosList() {
  const queryClient = useQueryClient()
  const toast = useToast()
  const [editando, setEditando] = useState<Usuario | null>(null)
  const [editError, setEditError] = useState('')
  const [confirmarDelete, setConfirmarDelete] = useState<number | null>(null)

  const { data: usuarios = [], isLoading, isError } = useQuery({
    queryKey: ['usuarios'],
    queryFn: getUsuarios,
  })

  const atualizar = useMutation({
    mutationFn: (u: Usuario) => atualizarUsuario(u.id, u),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['usuarios'] })
      setEditando(null)
      setEditError('')
      toast.success('Usuário atualizado.')
    },
    onError: (err) => {
      const axiosErr = err as AxiosError<{ message?: string; title?: string; errors?: Record<string, string[]> }>
      const message =
        axiosErr?.response?.data?.message ??
        axiosErr?.response?.data?.title ??
        Object.values(axiosErr?.response?.data?.errors ?? {}).flat()[0] ??
        'Erro ao atualizar usuário.'
      setEditError(message)
    },
  })

  const deletar = useMutation({
    mutationFn: deletarUsuario,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['usuarios'] })
      toast.success('Usuário excluído.')
    },
    onError: (err) => toast.error(err, 'Não foi possível excluir o usuário.'),
  })

  const abrirEdicao = (u: Usuario) => {
    setEditando({ ...u })
    setEditError('')
  }

  if (isLoading) return <Spinner />
  if (isError) return <p className="text-sm text-red-600">Erro ao carregar usuários.</p>

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold text-gray-900">Usuários</h2>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200">
            <tr className="text-left text-xs font-medium text-gray-500 uppercase tracking-wide">
              <th className="px-6 py-3">Nome</th>
              <th className="px-6 py-3">E-mail</th>
              <th className="px-6 py-3">Telefone</th>
              <th className="px-6 py-3">Status</th>
              <th className="px-6 py-3">Cadastro</th>
              <th className="px-6 py-3"></th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {usuarios.map((u) => (
              <tr key={u.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 font-medium text-gray-900">{u.nome}</td>
                <td className="px-6 py-4 text-gray-600">{u.email}</td>
                <td className="px-6 py-4 text-gray-600">{u.telefone || '—'}</td>
                <td className="px-6 py-4">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${u.ativo ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-500'}`}>
                    {u.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td className="px-6 py-4 text-gray-400">{new Date(u.dataCriacao).toLocaleDateString('pt-BR')}</td>
                <td className="px-6 py-4 flex gap-3">
                  <button onClick={() => abrirEdicao(u)} className="text-xs text-blue-600 hover:text-blue-700">Editar</button>
                  <button onClick={() => setConfirmarDelete(u.id)} className="text-xs text-red-600 hover:text-red-700">Excluir</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <ConfirmDialog
        isOpen={confirmarDelete !== null}
        title="Excluir usuário"
        message="Esta ação não pode ser desfeita. Deseja excluir este usuário?"
        confirmLabel="Excluir"
        onConfirm={() => { deletar.mutate(confirmarDelete!); setConfirmarDelete(null) }}
        onCancel={() => setConfirmarDelete(null)}
      />

      {editando && (
        <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl border border-gray-200 p-6 w-full max-w-md space-y-4">
            <h3 className="text-lg font-semibold text-gray-900">Editar usuário</h3>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Nome</label>
              <input
                value={editando.nome}
                onChange={(e) => setEditando({ ...editando, nome: e.target.value })}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">E-mail</label>
              <input
                type="email"
                value={editando.email}
                onChange={(e) => setEditando({ ...editando, email: e.target.value })}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Telefone</label>
              <input
                type="text"
                inputMode="numeric"
                placeholder="(00) 00000-0000"
                value={editando.telefone}
                onChange={(e) => setEditando({ ...editando, telefone: maskTelefone(e.target.value) })}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                checked={editando.ativo}
                onChange={(e) => setEditando({ ...editando, ativo: e.target.checked })}
                id="ativo"
              />
              <label htmlFor="ativo" className="text-sm text-gray-700">Ativo</label>
            </div>

            {editError && (
              <div className="rounded-lg bg-red-50 border border-red-200 px-4 py-3">
                <p className="text-sm text-red-700">{editError}</p>
              </div>
            )}

            <div className="flex gap-3 pt-2">
              <button
                onClick={() => { setEditando(null); setEditError('') }}
                className="px-4 py-2 text-sm border border-gray-300 rounded-lg text-gray-600"
              >
                Cancelar
              </button>
              <button
                onClick={() => atualizar.mutate(editando)}
                disabled={atualizar.isPending}
                className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg disabled:opacity-50"
              >
                {atualizar.isPending ? 'Salvando...' : 'Salvar'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
