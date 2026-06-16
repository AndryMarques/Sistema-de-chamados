interface Props {
  message?: string
}

export default function Spinner({ message = 'Carregando...' }: Props) {
  return (
    <div className="flex flex-col items-center justify-center py-16 gap-3">
      <div className="w-8 h-8 border-4 border-gray-200 border-t-blue-600 rounded-full animate-spin" />
      <p className="text-sm text-gray-400">{message}</p>
    </div>
  )
}
