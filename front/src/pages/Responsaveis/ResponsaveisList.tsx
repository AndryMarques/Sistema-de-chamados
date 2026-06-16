import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getResponsaveis, criarResponsavel, deletarResponsavel } from '../../api/responsaveis'
import { getUsuarios } from '../../api/usuarios'
import { useToast } from '../../hooks/useToast'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import Spinner from '../../components/ui/Spinner'

export default function ResponsaveisList() {
  const queryClient = useQueryClient()
  const toast = useToast()
  const [usuarioIdSelecionado, setUsuarioIdSelecionado] = useState<number | ''>('')
  const [confirmarDelete, setConfirmarDelete] = useState<number | null>(null)

  const { data: responsaveis = [], isLoading, isError } = useQuery({
    queryKey: ['responsaveis'],
    queryFn: getResponsaveis,
  })

  const { data: usuarios = [] } = useQuery({
    queryKey: ['usuarios'],
    queryFn: getUsuarios,
  })

  const criar = useMutation({
    mutationFn: () => criarResponsavel({ usuarioId: usuarioIdSelecionado as number }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['responsaveis'] })
      setUsuarioIdSelecionado('')
      toast.success('Usuário promovido a responsável.')
    },
    onError: (err) => toast.error(err, 'Erro ao promover usuário.'),
  })

  const deletar = useMutation({
    mutationFn: deletarResponsavel,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['responsaveis'] })
      toast.success('Responsável removido.')
    },
    onError: (err) => toast.error(err, 'Não foi possível remover o responsável.'),
  })

  const usuariosDisponiveis = usuarios.filter(
    (u) => !responsaveis.some((r) => r.usuarioId === u.id)
  )

  if (isLoading) return <Spinner />
  if (isError) return <p className="text-sm text-red-600">Erro ao carregar responsáveis.</p>

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold text-gray-900">Responsáveis</h2>

      {/* Promover usuário */}
      <div className="bg-white rounded-xl border border-gray-200 p-5">
        <h3 className="text-sm font-semibold text-gray-700 mb-3">Promover usuário a responsável</h3>
        <div className="flex gap-3">
          <select
            value={usuarioIdSelecionado}
            onChange={(e) => setUsuarioIdSelecionado(e.target.value === '' ? '' : Number(e.target.value))}
            className="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Selecione um usuário</option>
            {usuariosDisponiveis.map((u) => (
              <option key={u.id} value={u.id}>{u.nome} — {u.email}</option>
            ))}
          </select>
          <button
            onClick={() => criar.mutate()}
            disabled={usuarioIdSelecionado === '' || criar.isPending}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors disabled:opacity-50"
          >
            {criar.isPending ? 'Promovendo...' : 'Promover'}
          </button>
        </div>
      </div>

      {/* Lista de responsáveis */}
      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200">
            <tr className="text-left text-xs font-medium text-gray-500 uppercase tracking-wide">
              <th className="px-6 py-3">Nome</th>
              <th className="px-6 py-3">E-mail</th>
              <th className="px-6 py-3">Chamados em aberto</th>
              <th className="px-6 py-3">Desde</th>
              <th className="px-6 py-3"></th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {responsaveis.length === 0 ? (
              <tr>
                <td colSpan={5} className="px-6 py-4 text-gray-400">Nenhum responsável cadastrado.</td>
              </tr>
            ) : (
              responsaveis.map((r) => (
                <tr key={r.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-900">{r.usuario.nome}</td>
                  <td className="px-6 py-4 text-gray-600">{r.usuario.email}</td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${r.chamadosEmAberto === 0 ? 'bg-green-100 text-green-800' : r.chamadosEmAberto >= 5 ? 'bg-red-100 text-red-800' : 'bg-yellow-100 text-yellow-800'}`}>
                      {r.chamadosEmAberto}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-gray-400">{new Date(r.dataAssociacao).toLocaleDateString('pt-BR')}</td>
                  <td className="px-6 py-4">
                    <button
                      onClick={() => setConfirmarDelete(r.id)}
                      disabled={r.chamadosEmAberto > 0}
                      className="text-xs text-red-600 hover:text-red-700 disabled:opacity-30 disabled:cursor-not-allowed"
                      title={r.chamadosEmAberto > 0 ? 'Possui chamados em aberto' : 'Remover responsável'}
                    >
                      Remover
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
      <ConfirmDialog
        isOpen={confirmarDelete !== null}
        title="Remover responsável"
        message="Deseja remover este responsável? Ele não poderá mais receber chamados."
        confirmLabel="Remover"
        onConfirm={() => { deletar.mutate(confirmarDelete!); setConfirmarDelete(null) }}
        onCancel={() => setConfirmarDelete(null)}
      />
    </div>
  )
}
