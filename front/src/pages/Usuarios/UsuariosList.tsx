import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getUsuarios, atualizarUsuario, deletarUsuario } from '../../api/usuarios'
import type { Usuario } from '../../types'

export default function UsuariosList() {
  const queryClient = useQueryClient()
  const [editando, setEditando] = useState<Usuario | null>(null)

  const { data: usuarios = [], isLoading } = useQuery({
    queryKey: ['usuarios'],
    queryFn: getUsuarios,
  })

  const atualizar = useMutation({
    mutationFn: (u: Usuario) => atualizarUsuario(u.id, u),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['usuarios'] })
      setEditando(null)
    },
  })

  const deletar = useMutation({
    mutationFn: deletarUsuario,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['usuarios'] }),
  })

  if (isLoading) return <div className="text-gray-500 text-sm">Carregando...</div>

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
                <td className="px-6 py-4 text-gray-600">{u.telefone}</td>
                <td className="px-6 py-4">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${u.ativo ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-500'}`}>
                    {u.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td className="px-6 py-4 text-gray-400">{new Date(u.dataCriacao).toLocaleDateString('pt-BR')}</td>
                <td className="px-6 py-4 flex gap-3">
                  <button onClick={() => setEditando({ ...u })} className="text-xs text-blue-600 hover:text-blue-700">Editar</button>
                  <button onClick={() => deletar.mutate(u.id)} className="text-xs text-red-600 hover:text-red-700">Excluir</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal de edição inline */}
      {editando && (
        <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl border border-gray-200 p-6 w-full max-w-md space-y-4">
            <h3 className="text-lg font-semibold text-gray-900">Editar usuário</h3>
            {(['nome', 'email', 'telefone'] as const).map((field) => (
              <div key={field}>
                <label className="block text-sm font-medium text-gray-700 mb-1 capitalize">{field}</label>
                <input
                  value={editando[field]}
                  onChange={(e) => setEditando({ ...editando, [field]: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            ))}
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                checked={editando.ativo}
                onChange={(e) => setEditando({ ...editando, ativo: e.target.checked })}
                id="ativo"
              />
              <label htmlFor="ativo" className="text-sm text-gray-700">Ativo</label>
            </div>
            <div className="flex gap-3 pt-2">
              <button onClick={() => setEditando(null)} className="px-4 py-2 text-sm border border-gray-300 rounded-lg text-gray-600">Cancelar</button>
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
