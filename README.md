# REAP-6 — Ver perfil | Reaparece

Funcionalidad de **solo lectura** para el primer Sprint. En el tablero del Sprint, **REAP-6 Ver perfil** tiene **3 puntos**, diferente de las historias de registro e inicio de sesión asignadas a otros integrantes.

## Qué incluye

- **Frontend responsive:** tarjeta de usuario, correo privado, fecha de ingreso, estado de verificación, estadísticas, comunidades e historial de publicaciones.
- **Backend real:** servidor HTTP de Node.js, datos almacenados en SQLite, sesiones firmadas con HMAC SHA-256, rutas privadas.
- **Demo local:** usuario y objetos ficticios; no requiere crear cuentas reales ni depender de los módulos de registro/login que hacen tus compañeros.
- **Pruebas automatizadas** para permisos, datos y sesión.

**Importante:** es un módulo funcional de desarrollo, no una aplicación de producción terminada. Las políticas de recuperación, registro real, almacenamiento de fotografías y todas las demás historias del backlog están fuera del alcance de REAP-6.

## Cómo ejecutarlo

1. Instala **Node.js 22.13 o superior**. (Utiliza SQLite integrado; Node.js 22 puede mostrar una advertencia de característica experimental.)
2. Abre una terminal dentro de esta carpeta.
3. Ejecuta `npm start` (no necesita `npm install` porque no hay dependencias externas).
4. Abre **http://localhost:3000**.
5. Presiona **«Entrar con usuario de prueba»**.

Para ejecutar los tests: `npm test`.

Si el puerto está ocupado: `PORT=3001 npm start` en macOS/Linux; en PowerShell ` $env:PORT=3001; npm start `.

El archivo de base de datos se crea automáticamente en `data/reaparece.sqlite` cuando se ejecuta el servidor. Esta carpeta está excluida del proyecto comprimido y no debe subirse a Git.

## Rutas de API

| Método | Ruta | Acceso | Uso |
|---|---|---|---|
| GET | `/api/health` | Público | Comprobar el servicio |
| GET | `/api/me` | Sesión requerida | Datos del usuario autenticado y estadísticas |
| GET | `/api/me/objects` | Sesión requerida | Historial de publicaciones propias |
| POST | `/api/dev/session` | **Solo desarrollo** | Crear sesión de muestra |
| POST | `/api/dev/logout` | Desarrollo | Borrar cookie de prueba |

**Ejemplo de respuesta de `GET /api/me`** (parte del resultado):

```json
{
  "profile": {
    "id": "USR-DEMO-001",
    "fullName": "Usuario de demostración",
    "email": "demo@reaparece.local",
    "verified": true,
    "reputation": 20,
    "memberSince": "2026-10-01",
    "communities": [{ "id": "COM-DEMO-1", "name": "Comunidad universitaria", "category": "educativa", "role": "miembro" }],
    "stats": { "published": 4, "recovered": 2, "communities": 2 }
  }
}
```

## Conexión con lo que harán tus compañeros (REAP-2 Registro y REAP-3 Iniciar sesión)

**Tu módulo NO crea usuarios ni autentica contraseñas.** En desarrollo usa un botón de acceso ficticio para demostrar la funcionalidad. Cuando sus módulos de registro/login estén disponibles:

1. El registro inserta usuarios en `users` y las membresías en `memberships` (o adaptan las consultas a la base de datos consensuada).
2. El inicio de sesión verifica la contraseña **con bcrypt**, como exige la especificación del proyecto, y emite una sesión autenticada. Pueden usar la función `makeSessionToken(userId, secret)` del archivo `backend/auth.js` o reemplazar el middleware `getSessionUserId` por el sistema de autenticación del equipo.
3. La API `/api/me` **obtiene el ID desde la sesión, nunca desde un parámetro de URL que el usuario pueda cambiar**.
4. El frontend carga los datos con `fetch('/api/me')` y `fetch('/api/me/objects')`.
5. Quiten o deshabiliten el botón demo en producción y definan un `SESSION_SECRET` seguro (mínimo 32 caracteres) mediante variables de entorno. Para un sistema real, servir exclusivamente sobre HTTPS.

Ejemplo de integración desde el login (dentro del backend, después de verificar credenciales):

```js
const { makeSessionToken, sessionCookie } = require('./auth');
const token = makeSessionToken(user.id, process.env.SESSION_SECRET);
res.setHeader('Set-Cookie', sessionCookie(token, process.env.NODE_ENV === 'production'));
```

No se almacena ninguna contraseña de muestra; el flujo demo no equivale al inicio de sesión real. Nunca habilitar `/api/dev/session` en producción.

## Correspondencia con la especificación Reaparece.pro

- **RI-001:** ID, nombre, correo, comunidades, reputación e historial de objetos.
- **RI-002:** tipo de reporte y estado del objeto (`activo`, `en proceso`, `recuperado`, `no reclamado`).
- **RI-003:** comunidades y roles de membresía.
- **RNF-007:** el perfil es privado y solo responde para una sesión autenticada. No hay endpoint público para el correo. El módulo no implementa por sí solo cifrado en reposo ni gestión de claves ni verificación real de contraseñas.
- **Alcance de historia:** «Ver perfil»; NO «Editar perfil», «Registrarse» ni «Iniciar sesión».

## Posibles criterios de aceptación para presentar al equipo

1. Dado un usuario autenticado, cuando abre «Mi perfil», ve su nombre, correo, fecha de registro y estado de cuenta.
2. Se muestran las comunidades a las que pertenece y las publicaciones asociadas a su usuario.
3. Se presenta su reputación y las estadísticas sin incluir datos de otros usuarios.
4. Si no hay sesión válida, el backend responde con `401` y no entrega datos privados.
5. Si no tiene publicaciones o comunidades, se muestra un estado vacío entendible.
6. La vista se adapta a móvil y escritorio.

## Archivos

- `backend/db.js`: tablas SQLite, ejemplos de prueba, consultas.
- `backend/auth.js`: verificación y firma de tokens de sesión.
- `backend/server.js`: API y servidor web.
- `frontend/index.html`: interfaz de perfil.
- `frontend/styles.css`: diseño adaptable.
- `frontend/app.js`: llamadas a API y renderización segura de datos.
- `tests/profile.test.js`: pruebas automáticas.

---
**Guía rápida para sustentarlo:** «Implementé REAP-6 Ver perfil. El backend consulta la base de datos según el usuario autenticado y entrega su información y publicaciones por una API privada. El frontend consume esa API y muestra los datos, comunidades y reputación. Dejé la autenticación demo aislada para que se reemplace por el inicio de sesión del compañero; no incluyo registro ni edición de perfil porque corresponden a otras historias».
