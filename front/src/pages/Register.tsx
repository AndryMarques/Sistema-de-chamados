import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useState } from 'react'

const schema = z.object({
  nome: z.string().min(2, 'Nome obrigatório'),
  email: z.string().email('E-mail inválido'),
  senha: z.string().min(6, 'Mínimo 6 caracteres'),
  telefone: z.string().min(8, 'Telefone obrigatório'),
})
type FormData = z.infer<typeof schema>

export default function Register() {
  const { registrar, login } = useAuth()
  const navigate = useNavigate()
  const [error, setError] = useState('')

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const onSubmit = async (data: FormData) => {
    try {
      setError('')
      await registrar(data)
      await login({ email: data.email, senha: data.senha })
      navigate('/')
    } catch {
      setError('Erro ao criar conta. Verifique os dados.')
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="w-full max-w-sm bg-white rounded-xl shadow-sm border border-gray-200 p-8">
        <h1 className="text-2xl font-semibold text-gray-900 mb-6">Criar conta</h1>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {(['nome', 'email', 'telefone'] as const).map((field) => (
            <div key={field}>
              <label className="block text-sm font-medium text-gray-700 mb-1 capitalize">{field}</label>
              <input
                {...register(field)}
                type={field === 'email' ? 'email' : 'text'}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              {errors[field] && <p className="mt-1 text-xs text-red-600">{errors[field]?.message}</p>}
            </div>
          ))}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Senha</label>
            <input
              {...register('senha')}
              type="password"
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {errors.senha && <p className="mt-1 text-xs text-red-600">{errors.senha.message}</p>}
          </div>
          {error && <p className="text-sm text-red-600">{error}</p>}
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 rounded-lg text-sm transition-colors disabled:opacity-50"
          >
            {isSubmitting ? 'Criando...' : 'Criar conta'}
          </button>
        </form>
        <p className="mt-4 text-sm text-gray-500 text-center">
          Já tem conta?{' '}
          <Link to="/login" className="text-blue-600 hover:underline">Entrar</Link>
        </p>
      </div>
    </div>
  )
}
