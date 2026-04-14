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

## 8) Successful UI+API request (201 Created)

### Scenario: Add a new actor

**Frontend action:**
- User fills out the Add Actor form with:
  - Name: `"Tom Hanks"`
  - Birth Year: `1956`
  - Biography: `"American actor and filmmaker"`
- User clicks "Create Actor" button
- `Actors/add.js` calls `form.addEventListener('submit', ...)`

**Frontend sends (HTTP):**
```
POST http://localhost:8080/api/v1/actors HTTP/1.1
Content-Type: application/json
Accept: application/json

{
  "name": "Tom Hanks",
  "birthYear": 1956,
  "biography": "American actor and filmmaker"
}
```

**Backend processing:**

1. **Middleware pipeline:**
   - `StructuredLogging` logs the incoming request
   - `ParseRequestUrl` parses the URL
   - `ParseRequestQueryString` (none in this request)
   - `UseParametrizedRouteMatching` finds route match for `POST /actors`

2. **Route handler:**
   - `HttpUtils.ReadRequestBodyAsText` reads the JSON body into `props['req.text']`
   - `ActorsApiController.CreateActor` is invoked

3. **Controller:**
   ```csharp
   var text = (string)props["req.text"]!;
   var actor = JsonSerializer.Deserialize<Actor>(text, JsonSerializerOptions.Web);
   var result = await actorService.CreateActor(actor!);
   await JsonUtils.SendResultResponse(req, res, props, result);
   ```

4. **Service validation:**
   - `ActorsService.ValidateActor(actor)` checks:
     - name is not null/empty → ✓ `"Tom Hanks"`
     - name length <= 256 → ✓
     - birthYear between 1800 and current year → ✓ `1956`
   - All validations pass → returns `null` (no error)
   - Calls `repository.CreateActor(actor)`

5. **Repository:**
   - `actor.Id = db.NextActorId()` → assigns ID (e.g., `42`)
   - `db.Actors.Add(newActor)` → adds to in-memory list
   - returns the created actor with ID

6. **Response serialization:**
   ```csharp
   var result = new Result<Actor>(created, (int)HttpStatusCode.Created);
   await JsonUtils.SendResultResponse(req, res, props, result);
   ```
   - HTTP status: `201 Created`
   - Response body:
   ```json
   {
     "id": 42,
     "name": "Tom Hanks",
     "birthYear": 1956,
     "biography": "American actor and filmmaker"
   }
   ```

**Backend HTTP response:**
```
HTTP/1.1 201 Created
Content-Type: application/json
X-Request-Id: a1b2c3d4e5f6

{
  "id": 42,
  "name": "Tom Hanks",
  "birthYear": 1956,
  "biography": "American actor and filmmaker"
}
```

**Frontend handling:**
- `apiFetch()` receives response with `res.ok === true` (201 is in 200-299 range)
- Response body is parsed as JSON
- `add.js` receives `created` object
- Calls `renderStatus(statusEl, 'ok', 'Created actor #42 "Tom Hanks" (1956).')`
- Calls `form.reset()` to clear form fields
- User sees success message and form is ready for next entry

---

## 9) Unsuccessful UI+API request (400 Bad Request)

### Scenario: Attempt to add actor with empty name

**Frontend action:**
- User leaves Name field blank
- Fills Birth Year: `1990`
- Clicks "Create Actor"
- `Actors/add.js` calls `captureActorForm(form)`

**Frontend sends:**
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

**Backend processing:**

1. **Middleware pipeline:**
   - Same as before: logging, URL parsing, route matching

2. **Route handler:**
   - `ReadRequestBodyAsText` reads body
   - `ActorsApiController.CreateActor` is invoked

3. **Controller:**
   - Deserializes JSON to `Actor` object
   - Calls `actorService.CreateActor(actor)`

4. **Service validation:**
   - `ValidateActor(actor)` checks:
     - name is not null/empty → ✗ `""` is empty!
     - Returns error result immediately:
     ```csharp
     return new Result<Actor>(
       new Exception("Name is required and cannot be empty."),
       (int)HttpStatusCode.BadRequest
     );
     ```
   - Repository is never called

5. **Response serialization:**
   - `JsonUtils.SendResultResponse` detects `result.IsError == true`
   - Sets response status: `400 Bad Request`
   - Builds error JSON:
   ```csharp
   var jsonApiError = new { errors = new[] { result.Error! } };
   ```

**Backend HTTP response:**
```
HTTP/1.1 400 Bad Request
Content-Type: application/json
Cache-Control: no-store
X-Request-Id: x1y2z3a4b5c6

{
  "errors": [
    "Name is required and cannot be empty."
  ]
}
```

**Frontend handling:**
- `apiFetch()` receives response with `res.ok === false` (400 is not in 200-299 range)
- Throws an error in `common.js`:
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
- `add.js` catches error in `catch (err) { ... }` block
- Calls `renderStatus(statusEl, 'err', 'Create failed: Name is required and cannot be empty.')`
- User sees red error message on the page
- Form is NOT reset, so user can correct the data

---

## 10) Unsuccessful UI+API request (404 Not Found)

### Scenario: Attempt to delete a nonexistent actor

**Frontend action:**
- User is on Actors list page
- Somehow actor ID `999` is in the delete button (e.g., stale data)
- User clicks delete button
- `Actors/index.js` calls `apiFetch('/actors/999', { method: 'DELETE' })`

**Frontend sends:**
```
DELETE http://localhost:8080/api/v1/actors/999 HTTP/1.1
Accept: application/json
```

**Backend processing:**

1. **Middleware pipeline:**
   - `ParametrizedRouteMatching` matches `/actors/999` to `/actors/:id`
   - Extracts `id = 999` into `props['req.params']`

2. **Route handler:**
   - `ActorsApiController.DeleteActor` is invoked

3. **Controller:**
   ```csharp
   var uParams = (NameValueCollection)props["req.params"]!;
   int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
   var result = await actorService.DeleteActor(id);
   ```

4. **Service:**
   - Calls `repository.DeleteActor(999)`

5. **Repository:**
   - Searches `db.Actors` for actor with `id == 999`
   - `FirstOrDefault()` returns `null` (not found)
   - Returns `null` to service

6. **Service result:**
   ```csharp
   var deleted = await repository.DeleteActor(id);
   var result = deleted == null
     ? new Result<Actor>(
       new Exception($"Could not delete actor with id {id}."),
       (int)HttpStatusCode.NotFound
     )
     : new Result<Actor>(deleted, (int)HttpStatusCode.OK);
   ```
   - Returns `404 Not Found` error result

7. **Response serialization:**
   - `JsonUtils.SendResultResponse` detects error
   - Sets status: `404 Not Found`

**Backend HTTP response:**
```
HTTP/1.1 404 Not Found
Content-Type: application/json
Cache-Control: no-store
X-Request-Id: p1q2r3s4t5u6

{
  "errors": [
    "Could not delete actor with id 999."
  ]
}
```

**Frontend handling:**
- `apiFetch()` receives `res.ok === false` (404 is not in 200-299 range)
- Throws error with `err.status = 404`
- `Actors/index.js` catches error:
  ```js
  catch (err) {
    renderStatus(statusEl, 'err', `Delete failed: ${err.message}`);
  }
  ```
- User sees: `"Delete failed: Could not delete actor with id 999."`
- Page does NOT reload
- Actor list remains visible for user to try again

---

## 11) Error handling in middleware

### CentralizedErrorHandling

If an unexpected exception occurs anywhere in the pipeline:

```csharp
catch (Exception e) {
  int code = (int)HttpStatusCode.InternalServerError; // 500
  string message = Environment.GetEnvironmentVariable("DEPLOYMENT_MODE") == "production"
    ? "An unexpected error occurred."
    : e.ToString();
  await SendResponse(req, res, props, code, message, "text/plain");
}
```

- Development: shows full exception stack trace
- Production: generic message `"An unexpected error occurred."`

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
