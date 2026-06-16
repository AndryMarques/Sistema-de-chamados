import { useQuery } from '@tanstack/react-query'
import { getChamados } from '../api/chamados'
import { getResponsaveis } from '../api/responsaveis'
import { ChamadoStatus, ChamadoPrioridade } from '../types'
import Spinner from '../components/ui/Spinner'

const statusColors: Record<number, string> = {
  1: 'bg-blue-100 text-blue-800',
  2: 'bg-yellow-100 text-yellow-800',
  3: 'bg-green-100 text-green-800',
  4: 'bg-gray-100 text-gray-800',
  5: 'bg-purple-100 text-purple-800',
}

export default function Dashboard() {
  const { data: chamados = [], isLoading: loadingChamados } = useQuery({
    queryKey: ['chamados'],
    queryFn: getChamados,
  })

  const { data: responsaveis = [] } = useQuery({
    queryKey: ['responsaveis'],
    queryFn: getResponsaveis,
  })

  const contarPorStatus = (status: number) => chamados.filter((c) => c.status === status).length
  const contarPorPrioridade = (prioridade: number) => chamados.filter((c) => c.prioridade === prioridade).length

  if (loadingChamados) return <Spinner />

  return (
    <div className="space-y-8">
      <h2 className="text-2xl font-semibold text-gray-900">Dashboard</h2>

      {/* Cards de status */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl border border-gray-200 p-5">
          <p className="text-sm text-gray-500">Total de chamados</p>
          <p className="text-3xl font-bold text-gray-900 mt-1">{chamados.length}</p>
        </div>
        <div className="bg-white rounded-xl border border-gray-200 p-5">
          <p className="text-sm text-gray-500">Abertos</p>
          <p className="text-3xl font-bold text-blue-600 mt-1">{contarPorStatus(1)}</p>
        </div>
        <div className="bg-white rounded-xl border border-gray-200 p-5">
          <p className="text-sm text-gray-500">Em andamento</p>
          <p className="text-3xl font-bold text-yellow-600 mt-1">{contarPorStatus(2)}</p>
        </div>
        <div className="bg-white rounded-xl border border-gray-200 p-5">
          <p className="text-sm text-gray-500">Resolvidos</p>
          <p className="text-3xl font-bold text-green-600 mt-1">{contarPorStatus(3)}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Prioridade */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <h3 className="text-sm font-semibold text-gray-700 mb-4">Por prioridade</h3>
          <div className="space-y-3">
            {[3, 2, 1].map((p) => (
              <div key={p} className="flex items-center justify-between">
                <span className="text-sm text-gray-600">{ChamadoPrioridade[p]}</span>
                <span className="text-sm font-medium text-gray-900">{contarPorPrioridade(p)}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Carga dos responsáveis */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <h3 className="text-sm font-semibold text-gray-700 mb-4">Carga dos responsáveis</h3>
          {responsaveis.length === 0 ? (
            <p className="text-sm text-gray-400">Nenhum responsável cadastrado.</p>
          ) : (
            <div className="space-y-3">
              {responsaveis.slice(0, 5).map((r) => (
                <div key={r.id} className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">{r.usuario.nome}</span>
                  <span className="text-sm font-medium text-gray-900">
                    {r.chamadosEmAberto} aberto(s)
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Últimos chamados */}
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h3 className="text-sm font-semibold text-gray-700 mb-4">Chamados recentes</h3>
        {chamados.length === 0 ? (
          <p className="text-sm text-gray-400">Nenhum chamado encontrado.</p>
        ) : (
          <div className="divide-y divide-gray-100">
            {chamados.slice(0, 5).map((c) => (
              <div key={c.id} className="py-3 flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-900">{c.titulo}</p>
                  <p className="text-xs text-gray-400">{c.usuario?.nome}</p>
                </div>
                <span className={`text-xs px-2 py-1 rounded-full font-medium ${statusColors[c.status]}`}>
                  {ChamadoStatus[c.status]}
                </span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
