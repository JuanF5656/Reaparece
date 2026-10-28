interface Props {
  subtitulo?: string
}

export default function Encabezado({ subtitulo }: Props) {
  return (
    <header className="bg-bosque px-6 pt-10 pb-8 text-crema">
      <div className="mx-auto max-w-md">
        <h1 className="font-serif text-4xl font-bold tracking-tight">Reaparece</h1>
        {subtitulo && <p className="mt-2 text-sm text-crema/80">{subtitulo}</p>}
      </div>
    </header>
  )
}
