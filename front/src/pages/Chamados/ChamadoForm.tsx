import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { criarChamado } from '../../api/chamados'
import { getResponsaveis } from '../../api/responsaveis'
import { useAuth } from '../../context/AuthContext'
import { useToast } from '../../hooks/useToast'

const schema = z.object({
  titulo: z.string().min(3, 'Título obrigatório'),
  descricao: z.string().min(10, 'Descrição muito curta'),
  prioridade: z.number().min(1).max(3),
})
type FormData = z.infer<typeof schema>

export default function ChamadoForm() {
  const { usuario } = useAuth()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const toast = useToast()
  const [modoAtribuicao, setModoAtribuicao] = useState<'auto' | 'manual'>('auto')
  const [responsavelSelecionado, setResponsavelSelecionado] = useState<number | ''>('')

  const { data: responsaveis = [] } = useQuery({
    queryKey: ['responsaveis'],
    queryFn: getResponsaveis,
  })

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { prioridade: 1 },
  })

  const criar = useMutation({
    mutationFn: (data: FormData) =>
      criarChamado({
        ...data,
        usuarioId: usuario!.id,
        responsavelId: modoAtribuicao === 'manual' && responsavelSelecionado !== '' ? responsavelSelecionado : undefined,
      }),
    onSuccess: (chamado) => {
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      toast.success(
        modoAtribuicao === 'auto'
          ? 'Chamado criado e atribuído automaticamente.'
          : 'Chamado criado e atribuído ao responsável selecionado.'
      )
      navigate(`/chamados/${chamado.id}`)
    },
    onError: (err) => toast.error(err, 'Erro ao criar chamado.'),
  })

  const onSubmit = (data: FormData) => {
    if (modoAtribuicao === 'manual' && responsavelSelecionado === '') {
      return
    }
    criar.mutate(data)
  }

  return (
    <div className="max-w-xl space-y-6">
      <h2 className="text-2xl font-semibold text-gray-900">Novo chamado</h2>
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Título</label>
            <input
              {...register('titulo')}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {errors.titulo && <p className="mt-1 text-xs text-red-600">{errors.titulo.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Descrição</label>
            <textarea
              {...register('descricao')}
              rows={4}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />
            {errors.descricao && <p className="mt-1 text-xs text-red-600">{errors.descricao.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Prioridade</label>
            <select
              {...register('prioridade', { valueAsNumber: true })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value={1}>Baixa</option>
              <option value={2}>Média</option>
              <option value={3}>Alta</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Atribuição</label>
            <div className="flex gap-4 mb-3">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="modoAtribuicao"
                  checked={modoAtribuicao === 'auto'}
                  onChange={() => { setModoAtribuicao('auto'); setResponsavelSelecionado('') }}
                  className="accent-blue-600"
                />
                <span className="text-sm text-gray-700">Automático</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="modoAtribuicao"
                  checked={modoAtribuicao === 'manual'}
                  onChange={() => setModoAtribuicao('manual')}
                  className="accent-blue-600"
                />
                <span className="text-sm text-gray-700">Manual</span>
              </label>
            </div>

            {modoAtribuicao === 'auto' ? (
              <p className="text-xs text-gray-400">
                O chamado será atribuído ao responsável com menor carga de trabalho.
              </p>
            ) : (
              <div>
                <select
                  value={responsavelSelecionado}
                  onChange={(e) => setResponsavelSelecionado(e.target.value === '' ? '' : Number(e.target.value))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Selecione um responsável...</option>
                  {responsaveis.map((r) => (
                    <option key={r.id} value={r.id}>
                      {r.usuario?.nome ?? `Responsável #${r.id}`}
                      {' '}— {r.chamadosEmAberto} chamado{r.chamadosEmAberto !== 1 ? 's' : ''} em aberto
                    </option>
                  ))}
                </select>
                {responsaveis.length === 0 && (
                  <p className="mt-1 text-xs text-amber-600">Nenhum responsável cadastrado.</p>
                )}
                {modoAtribuicao === 'manual' && responsavelSelecionado === '' && (
                  <p className="mt-1 text-xs text-red-600">Selecione um responsável.</p>
                )}
              </div>
            )}
          </div>

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={() => navigate('/chamados')}
              className="px-4 py-2 text-sm font-medium text-gray-600 hover:text-gray-900 border border-gray-300 rounded-lg"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={isSubmitting || criar.isPending}
              className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors disabled:opacity-50"
            >
              {criar.isPending ? 'Criando...' : 'Criar chamado'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
