# Nota del Flujo de Solicitudes CSR

Esta nota documenta el flujo de extremo a extremo de las solicitudes del cliente en el proyecto `Smdb.Csr`, desde eventos de UI del navegador a través de JavaScript del lado del cliente, hasta la API del backend, y de vuelta al navegador.

## Resumen

El proyecto tiene dos servidores separados:

- `Smdb.Csr` sirve la UI del navegador estática y scripts del lado del cliente.
- `Smdb.Api` sirve la API REST JSON bajo `/api/v1`.

Una solicitud de carga de página del navegador es manejada por `Smdb.Csr`, mientras que las solicitudes `fetch()` de JS van a `Smdb.Api`.

---

## 1) Solicitud de página estática en CSR

### Ejemplo de ruta: `/actors`

1. El navegador solicita `GET /actors`
2. `Smdb.Csr/App.cs` enruta esto a una redirección a `/Actors/index.html`
3. El navegador solicita `/Actors/index.html`
4. `HttpUtils.ServeStaticFiles` sirve el HTML desde `wwwroot/Actors/index.html`
5. El navegador carga la página y su módulo JS `/scripts/Actors/index.js`

Archivo clave:
- `SimpleMDB/src/Smdb.Csr/App.cs`

---

## 2) Solicitud `fetch()` del frontend

### Disparador en `Actors/index.js`

- El script de la página calcula parámetros de paginación.
- Llama a `apiFetch('/actors?page=1&size=9')`.

### Ayudante compartido

- `SimpleMDB/src/Smdb.Csr/wwwroot/scripts/common.js`
- `API_BASE` es `http://localhost:8080/api/v1`
- `apiFetch()` construye la URL completa y envía `fetch()` con encabezados JSON.

Solicitud real al backend:
- `GET http://localhost:8080/api/v1/actors?page=1&size=9`

---

## 3) Enrutamiento del servidor API

### `Smdb.Api/App.cs`

Configura middleware y routers:

- `StructuredLogging`
- `CentralizedErrorHandling`
- `AddResponseCorsHeaders`
- `DefaultResponse`
- `ParseRequestUrl`
- `ParseRequestQueryString`
- `UseParametrizedRouteMatching()`

Luego monta routers bajo `/api/v1`:

- `/movies`
- `/actors`
- `/actormovie`
- `/auth`
- `/users`

### Router de actores

Archivo: `SimpleMDB/src/Smdb.Api/Actors/ActorsRouter.cs`

Rutas:
- `GET /` → `ReadActors`
- `POST /` → `CreateActor`
- `GET /:id` → `ReadActor`
- `PUT /:id` → `UpdateActor`
- `DELETE /:id` → `DeleteActor`

---

## 4) Manejo de solicitudes del backend

### Controlador

Archivo: `SimpleMDB/src/Smdb.Api/Actors/ActorsApiController.cs`

`ReadActors`:
- obtiene `page` y `size` de `req.QueryString`
- llama a `actorService.ReadActors(page, size)`
- envía el resultado con `JsonUtils.SendPagedResultResponse`

### Servicio

Archivo: `SimpleMDB/src/Smdb.Core/Actors/ActorsService.cs`

Valida entradas:
- `page >= 1`
- `size >= 1`

Luego llama a métodos del repositorio.

### Repositorio

Archivo: `SimpleMDB/src/Smdb.Core/Actors/ActorsRepository.cs`

Lee desde la base de datos en memoria:
- calcula `start` y `length`
- corta `db.Actors`
- devuelve `PagedResult<Actor>`

### Serialización de respuesta

Archivo: `SharedLibrary/src/Shared/Http/JsonUtils.cs`

`SendPagedResultResponse` construye JSON:

```json
{
  "data": [...],
  "meta": { "totalCount": 42, "page": 1, "size": 9, "totalPages": 5 },
  "links": { "self": "...", "next": "..." }
}
```

---

## 5) Renderizado del frontend

De vuelta en `Actors/index.js`:

- `payload.data` se lee
- se crean tarjetas de actores usando una plantilla
- se adjuntan enlaces de ver/editar y botones de eliminar

Aquí es donde el navegador recibe la respuesta y actualiza la página.

---

## 6) Ejemplo de otro evento del cliente: Agregar actor

### Evento del navegador

`wwwroot/Actors/add.js` escucha `submit` en el formulario de agregar.

### Acción

- `preventDefault()`
- `captureActorForm(form)` construye payload
- `apiFetch('/actors', { method: 'POST', body: JSON.stringify(payload) })`

### Ruta API

- `POST /api/v1/actors`
- `ActorsRouter.MapPost('/', HttpUtils.ReadRequestBodyAsText, apiController.CreateActor)`
- el cuerpo de la solicitud se convierte en `props['req.text']`
- el controlador deserializa JSON a `Actor`
- el servicio valida y el repositorio guarda
- la respuesta se envía con JSON del actor creado

### Resultado

El frontend muestra estado de éxito y resetea el formulario.

---

## 7) Flujo de eliminación

En `Actors/index.js`:
- clic en botón llama a `apiFetch('/actors/{id}', { method: 'DELETE' })`

Backend:
- `ActorsRouter.MapDelete('/:id', apiController.DeleteActor)`
- el controlador lee `req.params.id`
- el servicio y repositorio eliminan
- respuesta JSON devuelve éxito o error

---

## 8) Solicitud exitosa UI+API (201 Creado)

### Escenario: Agregar un nuevo actor

**Acción del frontend:**
- El usuario llena el formulario Agregar Actor con:
  - Nombre: `"Tom Hanks"`
  - Año de nacimiento: `1956`
  - Biografía: `"Actor y cineasta estadounidense"`
- El usuario hace clic en el botón "Crear Actor"
- `Actors/add.js` llama a `form.addEventListener('submit', ...)`

**El frontend envía (HTTP):**
```
POST http://localhost:8080/api/v1/actors HTTP/1.1
Content-Type: application/json
Accept: application/json

{
  "name": "Tom Hanks",
  "birthYear": 1956,
  "biography": "Actor y cineasta estadounidense"
}
```

**Procesamiento del backend:**

1. **Pipeline de middleware:**
   - `StructuredLogging` registra la solicitud entrante
   - `ParseRequestUrl` analiza la URL
   - `ParseRequestQueryString` (ninguna en esta solicitud)
   - `UseParametrizedRouteMatching` encuentra coincidencia de ruta para `POST /actors`

2. **Manejador de ruta:**
   - `HttpUtils.ReadRequestBodyAsText` lee el cuerpo JSON en `props['req.text']`
   - `ActorsApiController.CreateActor` se invoca

3. **Controlador:**
   ```csharp
   var text = (string)props["req.text"]!;
   var actor = JsonSerializer.Deserialize<Actor>(text, JsonSerializerOptions.Web);
   var result = await actorService.CreateActor(actor!);
   await JsonUtils.SendResultResponse(req, res, props, result);
   ```

4. **Validación del servicio:**
   - `ActorsService.ValidateActor(actor)` verifica:
     - nombre no es nulo/vacío → ✓ `"Tom Hanks"`
     - longitud del nombre <= 256 → ✓
     - birthYear entre 1800 y año actual → ✓ `1956`
   - Todas las validaciones pasan → devuelve `null` (sin error)
   - Llama a `repository.CreateActor(actor)`

5. **Repositorio:**
   - `actor.Id = db.NextActorId()` → asigna ID (ej. `42`)
   - `db.Actors.Add(newActor)` → agrega a la lista en memoria
   - devuelve el actor creado con ID

6. **Serialización de respuesta:**
   ```csharp
   var result = new Result<Actor>(created, (int)HttpStatusCode.Created);
   await JsonUtils.SendResultResponse(req, res, props, result);
   ```
   - Estado HTTP: `201 Created`
   - Cuerpo de respuesta:
   ```json
   {
     "id": 42,
     "name": "Tom Hanks",
     "birthYear": 1956,
     "biography": "Actor y cineasta estadounidense"
   }
   ```

**Respuesta HTTP del backend:**
```
HTTP/1.1 201 Created
Content-Type: application/json
X-Request-Id: a1b2c3d4e5f6

{
  "id": 42,
  "name": "Tom Hanks",
  "birthYear": 1956,
  "biography": "Actor y cineasta estadounidense"
}
```

**Manejo del frontend:**
- `apiFetch()` recibe respuesta con `res.ok === true` (201 está en rango 200-299)
- El cuerpo de respuesta se analiza como JSON
- `add.js` recibe objeto `created`
- Llama a `renderStatus(statusEl, 'ok', 'Actor creado #42 "Tom Hanks" (1956).')`
- Llama a `form.reset()` para limpiar los campos del formulario
- El usuario ve mensaje de éxito y el formulario está listo para la siguiente entrada

---

## 9) Solicitud no exitosa UI+API (400 Solicitud Incorrecta)

### Escenario: Intentar agregar actor con nombre vacío

**Acción del frontend:**
- El usuario deja el campo Nombre en blanco
- Llena Año de nacimiento: `1990`
- Hace clic en "Crear Actor"
- `Actors/add.js` llama a `captureActorForm(form)`

**El frontend envía:**
```
POST http://localhost:8080/api/v1/actors HTTP/1.1
Content-Type: application/json
Accept: application/json

{
  "name": "",
  "birthYear": 1990,
  "biography": ""
}
```

**Procesamiento del backend:**

1. **Pipeline de middleware:**
   - Igual que antes: logging, análisis de URL, coincidencia de ruta

2. **Manejador de ruta:**
   - `ReadRequestBodyAsText` lee el cuerpo
   - `ActorsApiController.CreateActor` se invoca

3. **Controlador:**
   - Deserializa JSON a objeto `Actor`
   - Llama a `actorService.CreateActor(actor)`

4. **Validación del servicio:**
   - `ValidateActor(actor)` verifica:
     - nombre no es nulo/vacío → ✗ `""` está vacío!
     - Devuelve resultado de error inmediatamente:
     ```csharp
     return new Result<Actor>(
       new Exception("El nombre es obligatorio y no puede estar vacío."),
       (int)HttpStatusCode.BadRequest
     );
     ```
   - El repositorio nunca se llama

5. **Serialización de respuesta:**
   - `JsonUtils.SendResultResponse` detecta `result.IsError == true`
   - Establece estado de respuesta: `400 Bad Request`
   - Construye JSON de error:
   ```csharp
   var jsonApiError = new { errors = new[] { result.Error! } };
   ```

**Respuesta HTTP del backend:**
```
HTTP/1.1 400 Bad Request
Content-Type: application/json
Cache-Control: no-store
X-Request-Id: x1y2z3a4b5c6

{
  "errors": [
    "El nombre es obligatorio y no puede estar vacío."
  ]
}
```

**Manejo del frontend:**
- `apiFetch()` recibe respuesta con `res.ok === false` (400 no está en rango 200-299)
- Lanza un error en `common.js`:
  ```js
  if (!res.ok) {
    const msg = (payload && (payload.message || payload.error)) ||
      `${res.status} ${res.statusText}`;
    const err = new Error(msg);
    err.status = res.status;
    err.payload = payload;
    throw err;
  }
  ```
- `add.js` atrapa el error en el bloque `catch (err) { ... }`
- Llama a `renderStatus(statusEl, 'err', 'Creación fallida: El nombre es obligatorio y no puede estar vacío.')`
- El usuario ve mensaje de error rojo en la página
- El formulario NO se resetea, por lo que el usuario puede corregir los datos

---

## 10) Solicitud no exitosa UI+API (404 No Encontrado)

### Escenario: Intentar eliminar un actor inexistente

**Acción del frontend:**
- El usuario está en la página de lista de Actores
- De alguna manera el ID de actor `999` está en el botón de eliminar (ej. datos obsoletos)
- El usuario hace clic en el botón de eliminar
- `Actors/index.js` llama a `apiFetch('/actors/999', { method: 'DELETE' })`

**El frontend envía:**
```
DELETE http://localhost:8080/api/v1/actors/999 HTTP/1.1
Accept: application/json
```

**Procesamiento del backend:**

1. **Pipeline de middleware:**
   - `ParametrizedRouteMatching` coincide `/actors/999` con `/actors/:id`
   - Extrae `id = 999` en `props['req.params']`

2. **Manejador de ruta:**
   - `ActorsApiController.DeleteActor` se invoca

3. **Controlador:**
   ```csharp
   var uParams = (NameValueCollection)props["req.params"]!;
   int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
   var result = await actorService.DeleteActor(id);
   ```

4. **Servicio:**
   - Llama a `repository.DeleteActor(999)`

5. **Repositorio:**
   - Busca en `db.Actors` actor con `id == 999`
   - `FirstOrDefault()` devuelve `null` (no encontrado)
   - Devuelve `null` al servicio

6. **Resultado del servicio:**
   ```csharp
   var deleted = await repository.DeleteActor(id);
   var result = deleted == null
     ? new Result<Actor>(
       new Exception($"No se pudo eliminar el actor con id {id}."),
       (int)HttpStatusCode.NotFound
     )
     : new Result<Actor>(deleted, (int)HttpStatusCode.OK);
   ```
   - Devuelve resultado de error `404 Not Found`

7. **Serialización de respuesta:**
   - `JsonUtils.SendResultResponse` detecta error
   - Establece estado: `404 Not Found`

**Respuesta HTTP del backend:**
```
HTTP/1.1 404 Not Found
Content-Type: application/json
Cache-Control: no-store
X-Request-Id: p1q2r3s4t5u6

{
  "errors": [
    "No se pudo eliminar el actor con id 999."
  ]
}
```

**Manejo del frontend:**
- `apiFetch()` recibe `res.ok === false` (404 no está en rango 200-299)
- Lanza error con `err.status = 404`
- `Actors/index.js` atrapa error:
  ```js
  catch (err) {
    renderStatus(statusEl, 'err', `Eliminación fallida: ${err.message}`);
  }
  ```
- El usuario ve: `"Eliminación fallida: No se pudo eliminar el actor con id 999."`
- La página NO se recarga
- La lista de actores permanece visible para que el usuario intente de nuevo

---

## 11) Manejo de errores en middleware

### CentralizedErrorHandling

Si ocurre una excepción inesperada en cualquier lugar del pipeline:

```csharp
catch (Exception e) {
  int code = (int)HttpStatusCode.InternalServerError; // 500
  string message = Environment.GetEnvironmentVariable("DEPLOYMENT_MODE") == "production"
    ? "Ocurrió un error inesperado."
    : e.ToString();
  await SendResponse(req, res, props, code, message, "text/plain");
}
```

- Desarrollo: muestra el rastreo completo de la pila de excepciones
- Producción: mensaje genérico `"Ocurrió un error inesperado."`

---

## Archivos clave para este flujo

- `SimpleMDB/src/Smdb.Csr/App.cs`
- `SimpleMDB/src/Smdb.Csr/wwwroot/scripts/common.js`
- `SimpleMDB/src/Smdb.Csr/wwwroot/scripts/Actors/index.js`
- `SimpleMDB/src/Smdb.Csr/wwwroot/scripts/Actors/add.js`
- `SimpleMDB/src/Smdb.Api/App.cs`
- `SimpleMDB/src/Smdb.Api/Actors/ActorsRouter.cs`
- `SimpleMDB/src/Smdb.Api/Actors/ActorsApiController.cs`
- `SimpleMDB/src/Smdb.Core/Actors/ActorsService.cs`
- `SimpleMDB/src/Smdb.Core/Actors/ActorsRepository.cs`
- `SharedLibrary/src/Shared/Http/JsonUtils.cs`
- `SharedLibrary/src/Shared/Http/HttpUtilities.cs`

---

## Nota

Esta nota se almacena en la raíz del repositorio y es puramente informativa. No modifica el comportamiento de la aplicación.
