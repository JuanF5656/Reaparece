import type { ReactNode } from 'react'

// Piezas de formulario reutilizables entre pantallas.

export const claseInput =
  'w-full rounded-xl border border-arena bg-white px-4 py-3 text-base outline-none transition focus:border-bosque focus:ring-2 focus:ring-bosque/20'

export const claseBoton =
  'w-full rounded-xl bg-bosque px-4 py-3 font-semibold text-white transition hover:bg-bosque-oscuro disabled:cursor-not-allowed disabled:opacity-50'

export function Campo({ etiqueta, children }: { etiqueta: string; children: ReactNode }) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-sm font-medium text-stone-700">{etiqueta}</span>
      {children}
    </label>
  )
}

export function MensajeError({ mensaje }: { mensaje: string | null }) {
  if (!mensaje) return null
  return (
    <p role="alert" className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
      {mensaje}
    </p>
  )
}
