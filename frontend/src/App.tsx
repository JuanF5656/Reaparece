import { useState } from 'react'
import Encabezado from './components/Encabezado'
import { claseBoton } from './components/formulario'
import Registro from './pages/Registro'
import type { Sesion } from './services/api'

export default function App() {
  // La sesión vive solo en memoria: al recargar la página se pierde.
  const [sesion, setSesion] = useState<Sesion | null>(null)

  if (!sesion) {
    return <Registro onRegistro={setSesion} />
  }

  return (
    <div className="min-h-svh">
      <Encabezado />
      <main className="mx-auto max-w-md space-y-6 px-4 py-6">
        <section className="rounded-2xl bg-white p-6 shadow-sm ring-1 ring-arena">
          <h2 className="font-serif text-2xl text-bosque">Hola, {sesion.usuario.nombreCompleto}</h2>
        </section>
        <button type="button" onClick={() => setSesion(null)} className={claseBoton}>
          Cerrar sesión
        </button>
      </main>
    </div>
  )
}
