import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getChamados, deletarChamado } from '../../api/chamados'
import { ChamadoStatus, ChamadoPrioridade } from '../../types'
import { useToast } from '../../hooks/useToast'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import Spinner from '../../components/ui/Spinner'

const statusColors: Record<number, string> = {
  1: 'bg-blue-100 text-blue-800',
  2: 'bg-yellow-100 text-yellow-800',
  3: 'bg-green-100 text-green-800',
  4: 'bg-gray-100 text-gray-800',
  5: 'bg-purple-100 text-purple-800',
}

const prioridadeColors: Record<number, string> = {
  1: 'text-green-600',
  2: 'text-yellow-600',
  3: 'text-red-600',
}

export default function ChamadosList() {
  const queryClient = useQueryClient()
  const toast = useToast()
  const [filtroStatus, setFiltroStatus] = useState<number | ''>('')
  const [confirmarDelete, setConfirmarDelete] = useState<number | null>(null)

  const { data: chamados = [], isLoading, isError } = useQuery({
    queryKey: ['chamados'],
    queryFn: getChamados,
  })

  const deletar = useMutation({
    mutationFn: deletarChamado,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      toast.success('Chamado excluído.')
    },
    onError: (err) => toast.error(err, 'Erro ao excluir chamado.'),
  })

  const lista = filtroStatus === '' ? chamados : chamados.filter((c) => c.status === filtroStatus)

  if (isLoading) return <Spinner />
  if (isError) return <p className="text-sm text-red-600">Erro ao carregar chamados.</p>

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-gray-900">Chamados</h2>
        <Link
          to="/chamados/novo"
          className="bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors"
        >
          Novo chamado
        </Link>
      </div>

      <div className="flex gap-2 flex-wrap">
        <button
          onClick={() => setFiltroStatus('')}
          className={`px-3 py-1 rounded-full text-xs font-medium transition-colors ${filtroStatus === '' ? 'bg-gray-900 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'}`}
        >
          Todos ({chamados.length})
        </button>
        {Object.entries(ChamadoStatus).map(([key, label]) => {
          const k = Number(key)
          return (
            <button
              key={k}
              onClick={() => setFiltroStatus(k)}
              className={`px-3 py-1 rounded-full text-xs font-medium transition-colors ${filtroStatus === k ? 'bg-gray-900 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'}`}
            >
              {label} ({chamados.filter((c) => c.status === k).length})
            </button>
          )
        })}
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        {lista.length === 0 ? (
          <p className="p-6 text-sm text-gray-400">Nenhum chamado encontrado.</p>
        ) : (
          <table className="w-full text-sm">
            <thead className="border-b border-gray-200">
              <tr className="text-left text-xs font-medium text-gray-500 uppercase tracking-wide">
                <th className="px-6 py-3">Título</th>
                <th className="px-6 py-3">Status</th>
                <th className="px-6 py-3">Prioridade</th>
                <th className="px-6 py-3">Usuário</th>
                <th className="px-6 py-3">Responsável</th>
                <th className="px-6 py-3">Abertura</th>
                <th className="px-6 py-3"></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {lista.map((c) => (
                <tr key={c.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-900">
                    <Link to={`/chamados/${c.id}`} className="hover:text-blue-600">{c.titulo}</Link>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[c.status]}`}>
                      {ChamadoStatus[c.status]}
                    </span>
                  </td>
                  <td className={`px-6 py-4 font-medium ${prioridadeColors[c.prioridade]}`}>
                    {ChamadoPrioridade[c.prioridade]}
                  </td>
                  <td className="px-6 py-4 text-gray-600">{c.usuario?.nome ?? '-'}</td>
                  <td className="px-6 py-4 text-gray-600">{c.responsavel?.usuario?.nome ?? 'Não atribuído'}</td>
                  <td className="px-6 py-4 text-gray-400">
                    {new Date(c.dataAbertura).toLocaleDateString('pt-BR')}
                  </td>
                  <td className="px-6 py-4">
                    {c.status === 4 && (
                      <button
                        onClick={() => setConfirmarDelete(c.id)}
                        className="text-xs text-red-600 hover:text-red-700"
                      >
                        Excluir
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
      <ConfirmDialog
        isOpen={confirmarDelete !== null}
        title="Excluir chamado"
        message="Esta ação não pode ser desfeita. Deseja excluir este chamado?"
        confirmLabel="Excluir"
        onConfirm={() => { deletar.mutate(confirmarDelete!); setConfirmarDelete(null) }}
        onCancel={() => setConfirmarDelete(null)}
      />
    </div>
  )
}
