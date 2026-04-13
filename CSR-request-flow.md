# CSR Request Flow Note

This note documents the end-to-end flow of client requests in the `Smdb.Csr` project, from browser UI events through front-end JavaScript, to the backend API, and back to the browser.

## Overview

The project has two separate servers:

- `Smdb.Csr` serves the static browser UI and client-side scripts.
- `Smdb.Api` serves the JSON REST API under `/api/v1`.

A browser page load request is handled by `Smdb.Csr`, while `fetch()` requests from JS go to `Smdb.Api`.

---

## 1) Static page request in CSR

### Path example: `/actors`

1. Browser requests `GET /actors`
2. `Smdb.Csr/App.cs` routes this to a redirect to `/Actors/index.html`
3. Browser requests `/Actors/index.html`
4. `HttpUtils.ServeStaticFiles` serves the HTML from `wwwroot/Actors/index.html`
5. Browser loads the page and its JS module `/scripts/Actors/index.js`

Key file:
- `SimpleMDB/src/Smdb.Csr/App.cs`

---

## 2) Frontend `fetch()` request

### Trigger in `Actors/index.js`

- The page script computes paging parameters.
- It calls `apiFetch('/actors?page=1&size=9')`.

### Shared helper

- `SimpleMDB/src/Smdb.Csr/wwwroot/scripts/common.js`
- `API_BASE` is `http://localhost:8080/api/v1`
- `apiFetch()` builds the full URL and sends `fetch()` with JSON headers.

Actual backend request:
- `GET http://localhost:8080/api/v1/actors?page=1&size=9`

---

## 3) API server routing

### `Smdb.Api/App.cs`

It sets up middleware and routers:

- `StructuredLogging`
- `CentralizedErrorHandling`
- `AddResponseCorsHeaders`
- `DefaultResponse`
- `ParseRequestUrl`
- `ParseRequestQueryString`
- `UseParametrizedRouteMatching()`

Then it mounts routers under `/api/v1`:

- `/movies`
- `/actors`
- `/actormovie`
- `/auth`
- `/users`

### Actor router

File: `SimpleMDB/src/Smdb.Api/Actors/ActorsRouter.cs`

Routes:
- `GET /` → `ReadActors`
- `POST /` → `CreateActor`
- `GET /:id` → `ReadActor`
- `PUT /:id` → `UpdateActor`
- `DELETE /:id` → `DeleteActor`

---

## 4) Backend request handling

### Controller

File: `SimpleMDB/src/Smdb.Api/Actors/ActorsApiController.cs`

`ReadActors`:
- gets `page` and `size` from `req.QueryString`
- calls `actorService.ReadActors(page, size)`
- sends the result with `JsonUtils.SendPagedResultResponse`

### Service

File: `SimpleMDB/src/Smdb.Core/Actors/ActorsService.cs`

Validates inputs:
- `page >= 1`
- `size >= 1`

Then calls repository methods.

### Repository

File: `SimpleMDB/src/Smdb.Core/Actors/ActorsRepository.cs`

Reads from the in-memory database:
- calculates `start` and `length`
- slices `db.Actors`
- returns `PagedResult<Actor>`

### Response serialization

File: `SharedLibrary/src/Shared/Http/JsonUtils.cs`

`SendPagedResultResponse` builds JSON:

```json
{
  "data": [...],
  "meta": { "totalCount": 42, "page": 1, "size": 9, "totalPages": 5 },
  "links": { "self": "...", "next": "..." }
}
```

---

## 5) Frontend rendering

Back in `Actors/index.js`:

- `payload.data` is read
- actor cards are created using a template
- view/edit links and delete buttons are attached

This is where the browser receives the response and updates the page.

---

## 6) Example of another client event: Add actor

### Browser event

`wwwroot/Actors/add.js` listens for `submit` on the add form.

### Action

- `preventDefault()`
- `captureActorForm(form)` builds payload
- `apiFetch('/actors', { method: 'POST', body: JSON.stringify(payload) })`

### API path

- `POST /api/v1/actors`
- `ActorsRouter.MapPost('/', HttpUtils.ReadRequestBodyAsText, apiController.CreateActor)`
- request body becomes `props['req.text']`
- controller deserializes JSON to `Actor`
- service validates and repository saves
- response is sent with created actor JSON

### Result

Frontend shows success status and resets the form.

---

## 7) Delete flow

In `Actors/index.js`:
- button click calls `apiFetch('/actors/{id}', { method: 'DELETE' })`

Backend:
- `ActorsRouter.MapDelete('/:id', apiController.DeleteActor)`
- controller reads `req.params.id`
- service and repository delete
- JSON response returns success or error

---

## Key files for this flow

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

## Note

This note is stored in the repository root and is purely informational. It does not modify application behavior.
