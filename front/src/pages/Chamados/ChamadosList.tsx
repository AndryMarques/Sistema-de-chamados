import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getChamados, atualizarChamado, deletarChamado } from '../../api/chamados'
import { getResponsaveis, obterOuCriarResponsavel } from '../../api/responsaveis'
import { criarAcompanhamento } from '../../api/acompanhamentos'
import { ChamadoStatus, ChamadoPrioridade } from '../../types'
import type { Chamado } from '../../types'
import { useAuth } from '../../context/AuthContext'
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
  const { usuario } = useAuth()
  const [filtroStatus, setFiltroStatus] = useState<number | ''>('')
  const [confirmarDelete, setConfirmarDelete] = useState<number | null>(null)
  const [finalizarChamado, setFinalizarChamado] = useState<Chamado | null>(null)
  const [justificativa, setJustificativa] = useState('')
  const [statusFinalizar, setStatusFinalizar] = useState<3 | 4>(3)

  const { data: chamados = [], isLoading, isError } = useQuery({
    queryKey: ['chamados'],
    queryFn: getChamados,
  })

  const { data: responsaveis = [] } = useQuery({
    queryKey: ['responsaveis'],
    queryFn: getResponsaveis,
  })

  const meuResponsavel = responsaveis.find((r) => r.usuarioId === usuario?.id)

  const deletar = useMutation({
    mutationFn: deletarChamado,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      toast.success('Chamado excluído.')
    },
    onError: (err) => toast.error(err, 'Erro ao excluir chamado.'),
  })

  const capturar = useMutation({
    mutationFn: async (c: Chamado) => {
      const responsavel = await obterOuCriarResponsavel(usuario!.id)
      return atualizarChamado(c.id, {
        id: c.id,
        titulo: c.titulo,
        descricao: c.descricao,
        prioridade: c.prioridade,
        status: 2,
        responsavelId: responsavel.id,
      })
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      queryClient.invalidateQueries({ queryKey: ['responsaveis'] })
      toast.success('Chamado capturado. Status alterado para Em Andamento.')
    },
    onError: (err) => toast.error(err, 'Erro ao capturar chamado.'),
  })

  const finalizar = useMutation({
    mutationFn: async ({ chamado, texto, status }: { chamado: Chamado; texto: string; status: 3 | 4 }) => {
      await criarAcompanhamento({
        chamadoId: chamado.id,
        responsavelId: meuResponsavel!.id,
        descricao: texto,
      })
      await atualizarChamado(chamado.id, {
        id: chamado.id,
        titulo: chamado.titulo,
        descricao: chamado.descricao,
        prioridade: chamado.prioridade,
        status,
        responsavelId: chamado.responsavelId,
      })
    },
    onSuccess: (_, vars) => {
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      queryClient.invalidateQueries({ queryKey: ['responsaveis'] })
      setFinalizarChamado(null)
      setJustificativa('')
      setStatusFinalizar(3)
      toast.success(vars.status === 4 ? 'Chamado fechado com sucesso.' : 'Chamado resolvido com sucesso.')
    },
    onError: (err) => toast.error(err, 'Erro ao finalizar chamado.'),
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
                <th className="px-6 py-3">Ação</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {lista.map((c) => {
                const euSouResponsavel = meuResponsavel && c.responsavelId === meuResponsavel.id
                const semResponsavel = !c.responsavelId
                const possoCapturar = !!usuario && (c.status === 1 || c.status === 2) && (semResponsavel || (!euSouResponsavel))
                const possoFinalizar = !!euSouResponsavel && (c.status === 1 || c.status === 2)

                return (
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
                    <td className="px-6 py-4 text-gray-600">{c.usuario?.nome ?? '—'}</td>
                    <td className="px-6 py-4 text-gray-600">{c.responsavel?.usuario?.nome ?? 'Não atribuído'}</td>
                    <td className="px-6 py-4 text-gray-400">
                      {new Date(c.dataAbertura).toLocaleDateString('pt-BR')}
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex gap-2 flex-wrap">
                        {possoCapturar && (
                          <button
                            onClick={() => capturar.mutate(c)}
                            disabled={capturar.isPending}
                            className="px-3 py-1 text-xs font-medium rounded-md bg-blue-600 hover:bg-blue-700 text-white disabled:opacity-50 transition-colors"
                          >
                            {capturar.isPending ? '...' : 'Capturar'}
                          </button>
                        )}
                        {possoFinalizar && (
                          <button
                            onClick={() => { setFinalizarChamado(c); setJustificativa(''); setStatusFinalizar(3) }}
                            className="px-3 py-1 text-xs font-medium rounded-md bg-green-600 hover:bg-green-700 text-white transition-colors"
                          >
                            Finalizar
                          </button>
                        )}
                        {c.status === 4 && (
                          <button
                            onClick={() => setConfirmarDelete(c.id)}
                            className="px-3 py-1 text-xs font-medium rounded-md bg-red-100 hover:bg-red-200 text-red-700 transition-colors"
                          >
                            Excluir
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                )
              })}
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

      {finalizarChamado && (
        <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl border border-gray-200 p-6 w-full max-w-md space-y-4">
            <h3 className="text-lg font-semibold text-gray-900">Finalizar chamado</h3>
            <p className="text-sm text-gray-500">
              <span className="font-medium text-gray-900">"{finalizarChamado.titulo}"</span>
            </p>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Status de encerramento</label>
              <div className="flex gap-4">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="radio"
                    name="statusFinalizar"
                    checked={statusFinalizar === 3}
                    onChange={() => setStatusFinalizar(3)}
                    className="accent-green-600"
                  />
                  <span className="text-sm text-gray-700">Resolvido</span>
                </label>
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="radio"
                    name="statusFinalizar"
                    checked={statusFinalizar === 4}
                    onChange={() => setStatusFinalizar(4)}
                    className="accent-gray-600"
                  />
                  <span className="text-sm text-gray-700">Fechado</span>
                </label>
              </div>
              <p className="mt-1 text-xs text-gray-400">
                {statusFinalizar === 3
                  ? 'Problema solucionado — aguarda confirmação do usuário.'
                  : 'Chamado encerrado definitivamente sem retorno esperado.'}
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Justificativa <span className="text-red-500">*</span>
              </label>
              <textarea
                value={justificativa}
                onChange={(e) => setJustificativa(e.target.value)}
                rows={4}
                placeholder="Descreva como o chamado foi resolvido..."
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
              />
              {justificativa.trim().length === 0 && justificativa.length > 0 && (
                <p className="mt-1 text-xs text-red-600">A justificativa não pode ser vazia.</p>
              )}
            </div>

            <div className="flex gap-3 justify-end">
              <button
                onClick={() => { setFinalizarChamado(null); setJustificativa(''); setStatusFinalizar(3) }}
                className="px-4 py-2 text-sm border border-gray-300 rounded-lg text-gray-600 hover:bg-gray-50"
              >
                Cancelar
              </button>
              <button
                onClick={() => finalizar.mutate({ chamado: finalizarChamado, texto: justificativa, status: statusFinalizar })}
                disabled={finalizar.isPending || !justificativa.trim()}
                className={`px-4 py-2 text-white text-sm font-medium rounded-lg disabled:opacity-50 transition-colors ${statusFinalizar === 4 ? 'bg-gray-600 hover:bg-gray-700' : 'bg-green-600 hover:bg-green-700'}`}
              >
                {finalizar.isPending
                  ? 'Finalizando...'
                  : statusFinalizar === 4 ? 'Confirmar fechamento' : 'Confirmar resolução'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
