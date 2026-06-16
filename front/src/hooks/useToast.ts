import { toast } from 'sonner'
import type { AxiosError } from 'axios'

export function useToast() {
  const success = (message: string) => toast.success(message)

  const error = (err: unknown, fallback = 'Ocorreu um erro inesperado.') => {
    const axiosErr = err as AxiosError<{ message?: string }>
    const message = axiosErr?.response?.data?.message ?? fallback
    toast.error(message)
  }

  return { success, error }
}
