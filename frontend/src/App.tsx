import { useState } from 'react'
import Login from './pages/Login'
import Perfil from './pages/Perfil'
import Registro from './pages/Registro'
import type { Usuario } from './services/api'

type PantallaPublica = 'login' | 'registro'

export default function App() {
  // La sesión vive solo en memoria: al recargar la página hay que volver a ingresar.
  const [usuario, setUsuario] = useState<Usuario | null>(null)
  const [pantalla, setPantalla] = useState<PantallaPublica>('login')

  if (usuario) {
    return (
      <Perfil
        usuario={usuario}
        onCerrarSesion={() => {
          setUsuario(null)
          setPantalla('login')
        }}
      />
    )
  }

  return pantalla === 'login' ? (
    <Login onIngreso={setUsuario} onIrARegistro={() => setPantalla('registro')} />
  ) : (
    <Registro onRegistro={setUsuario} onIrALogin={() => setPantalla('login')} />
  )
}
