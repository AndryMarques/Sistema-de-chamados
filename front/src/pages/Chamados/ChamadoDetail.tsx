import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { getChamado, atualizarChamado } from '../../api/chamados'
import { criarAcompanhamento, deletarAcompanhamento } from '../../api/acompanhamentos'
import { getResponsaveis } from '../../api/responsaveis'
import { ChamadoStatus, ChamadoPrioridade } from '../../types'

const statusColors: Record<number, string> = {
  1: 'bg-blue-100 text-blue-800',
  2: 'bg-yellow-100 text-yellow-800',
  3: 'bg-green-100 text-green-800',
  4: 'bg-gray-100 text-gray-800',
  5: 'bg-purple-100 text-purple-800',
}

const acompSchema = z.object({
  responsavelId: z.number().min(1, 'Selecione um responsável'),
  descricao: z.string().min(5, 'Descrição obrigatória'),
})
type AcompForm = z.infer<typeof acompSchema>

export default function ChamadoDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [editando, setEditando] = useState(false)
  const [novoStatus, setNovoStatus] = useState<number>(1)
  const [novoResponsavel, setNovoResponsavel] = useState<number | ''>('')

  const { data: chamado, isLoading } = useQuery({
    queryKey: ['chamado', id],
    queryFn: () => getChamado(Number(id)),
  })

  const { data: responsaveis = [] } = useQuery({
    queryKey: ['responsaveis'],
    queryFn: getResponsaveis,
  })

  const atualizar = useMutation({
    mutationFn: () =>
      atualizarChamado(Number(id), {
        id: Number(id),
        titulo: chamado!.titulo,
        descricao: chamado!.descricao,
        prioridade: chamado!.prioridade,
        status: novoStatus || chamado!.status,
        responsavelId: novoResponsavel !== '' ? novoResponsavel : chamado!.responsavelId,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['chamado', id] })
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      setEditando(false)
    },
  })

  const { register, handleSubmit, reset, formState: { errors } } = useForm<AcompForm>({
    resolver: zodResolver(acompSchema),
  })

  const addAcomp = useMutation({
    mutationFn: (data: AcompForm) =>
      criarAcompanhamento({ ...data, chamadoId: Number(id) }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['chamado', id] })
      reset()
    },
  })

  const delAcomp = useMutation({
    mutationFn: deletarAcompanhamento,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['chamado', id] }),
  })

  if (isLoading) return <div className="text-gray-500 text-sm">Carregando...</div>
  if (!chamado) return <div className="text-gray-500 text-sm">Chamado não encontrado.</div>

  return (
    <div className="max-w-3xl space-y-6">
      <div className="flex items-center gap-4">
        <button onClick={() => navigate('/chamados')} className="text-sm text-gray-500 hover:text-gray-900">
          ← Voltar
        </button>
        <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[chamado.status]}`}>
          {ChamadoStatus[chamado.status]}
        </span>
      </div>

      {/* Detalhes do chamado */}
      <div className="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
        <div className="flex items-start justify-between">
          <h2 className="text-xl font-semibold text-gray-900">{chamado.titulo}</h2>
          <button
            onClick={() => { setEditando(!editando); setNovoStatus(chamado!.status); setNovoResponsavel(chamado!.responsavelId ?? '') }}
            className="text-sm text-blue-600 hover:text-blue-700"
          >
            {editando ? 'Cancelar' : 'Editar'}
          </button>
        </div>
        <p className="text-sm text-gray-600">{chamado.descricao}</p>
        <div className="grid grid-cols-2 gap-4 text-sm">
          <div>
            <span className="text-gray-400">Prioridade: </span>
            <span className="font-medium">{ChamadoPrioridade[chamado.prioridade]}</span>
          </div>
          <div>
            <span className="text-gray-400">Usuário: </span>
            <span className="font-medium">{chamado.usuario?.nome ?? '-'}</span>
          </div>
          <div>
            <span className="text-gray-400">Responsável: </span>
            <span className="font-medium">{chamado.responsavel?.usuario?.nome ?? 'Não atribuído'}</span>
          </div>
          <div>
            <span className="text-gray-400">Abertura: </span>
            <span className="font-medium">{new Date(chamado.dataAbertura).toLocaleDateString('pt-BR')}</span>
          </div>
          {chamado.dataResolucao && (
            <div>
              <span className="text-gray-400">Resolução: </span>
              <span className="font-medium">{new Date(chamado.dataResolucao).toLocaleDateString('pt-BR')}</span>
            </div>
          )}
          {chamado.dataEncerramento && (
            <div>
              <span className="text-gray-400">Encerramento: </span>
              <span className="font-medium">{new Date(chamado.dataEncerramento).toLocaleDateString('pt-BR')}</span>
            </div>
          )}
        </div>

        {editando && (
          <div className="border-t border-gray-100 pt-4 space-y-3">
            <div>
              <label className="block text-xs font-medium text-gray-700 mb-1">Status</label>
              <select
                value={novoStatus}
                onChange={(e) => setNovoStatus(Number(e.target.value))}
                className="border border-gray-300 rounded-lg px-3 py-2 text-sm w-full focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                {Object.entries(ChamadoStatus).map(([k, v]) => (
                  <option key={k} value={k}>{v}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-700 mb-1">Responsável</label>
              <select
                value={novoResponsavel}
                onChange={(e) => setNovoResponsavel(e.target.value === '' ? '' : Number(e.target.value))}
                className="border border-gray-300 rounded-lg px-3 py-2 text-sm w-full focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Manter atual</option>
                {responsaveis.map((r) => (
                  <option key={r.id} value={r.id}>{r.usuario.nome} ({r.chamadosEmAberto} abertos)</option>
                ))}
              </select>
            </div>
            <button
              onClick={() => atualizar.mutate()}
              disabled={atualizar.isPending}
              className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors disabled:opacity-50"
            >
              {atualizar.isPending ? 'Salvando...' : 'Salvar'}
            </button>
          </div>
        )}
      </div>

      {/* Acompanhamentos */}
      <div className="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
        <h3 className="text-sm font-semibold text-gray-700">Acompanhamentos ({chamado.acompanhamentos?.length ?? 0})</h3>

        {(chamado.acompanhamentos ?? []).length === 0 ? (
          <p className="text-sm text-gray-400">Nenhum acompanhamento registrado.</p>
        ) : (
          <div className="space-y-3">
            {chamado.acompanhamentos!.map((a) => (
              <div key={a.id} className="border border-gray-100 rounded-lg p-4">
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-xs text-gray-400 mb-1">
                      {a.responsavel?.usuario?.nome} · {new Date(a.dataAcompanhamento).toLocaleDateString('pt-BR')}
                    </p>
                    <p className="text-sm text-gray-700">{a.descricao}</p>
                  </div>
                  <button
                    onClick={() => delAcomp.mutate(a.id)}
                    className="text-xs text-red-500 hover:text-red-700 ml-4"
                  >
                    Excluir
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}

        <form onSubmit={handleSubmit((d) => addAcomp.mutate(d))} className="border-t border-gray-100 pt-4 space-y-3">
          <p className="text-xs font-medium text-gray-700">Adicionar acompanhamento</p>
          <div>
            <select
              {...register('responsavelId', { valueAsNumber: true })}
              className="border border-gray-300 rounded-lg px-3 py-2 text-sm w-full focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value={0}>Selecione o responsável</option>
              {responsaveis.map((r) => (
                <option key={r.id} value={r.id}>{r.usuario.nome}</option>
              ))}
            </select>
            {errors.responsavelId && <p className="mt-1 text-xs text-red-600">{errors.responsavelId.message}</p>}
          </div>
          <div>
            <textarea
              {...register('descricao')}
              rows={3}
              placeholder="Descreva o que foi feito..."
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />
            {errors.descricao && <p className="mt-1 text-xs text-red-600">{errors.descricao.message}</p>}
          </div>
          <button
            type="submit"
            disabled={addAcomp.isPending}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors disabled:opacity-50"
          >
            {addAcomp.isPending ? 'Adicionando...' : 'Adicionar'}
          </button>
        </form>
      </div>
    </div>
  )
}
