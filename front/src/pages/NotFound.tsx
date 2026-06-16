import { Link } from 'react-router-dom'

export default function NotFound() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 text-center px-4">
      <p className="text-6xl font-bold text-gray-200">404</p>
      <h1 className="mt-4 text-xl font-semibold text-gray-900">Página não encontrada</h1>
      <p className="mt-2 text-sm text-gray-500">O endereço que você acessou não existe.</p>
      <Link
        to="/"
        className="mt-6 px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors"
      >
        Voltar ao início
      </Link>
    </div>
  )
}
