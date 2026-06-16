import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { criarChamado } from '../../api/chamados'
import { useAuth } from '../../context/AuthContext'

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

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { prioridade: 1 },
  })

  const criar = useMutation({
    mutationFn: (data: FormData) =>
      criarChamado({ ...data, usuarioId: usuario!.id }),
    onSuccess: (chamado) => {
      queryClient.invalidateQueries({ queryKey: ['chamados'] })
      navigate(`/chamados/${chamado.id}`)
    },
  })

  const onSubmit = (data: FormData) => criar.mutate(data)

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
          {criar.isError && (
            <p className="text-sm text-red-600">Erro ao criar chamado.</p>
          )}
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
      <p className="text-xs text-gray-400">
        O chamado será atribuído automaticamente ao responsável com menor carga.
      </p>
    </div>
  )
}
