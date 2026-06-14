# Num2Words - Varun Rao

Convert currency amounts to words in **English** and **German**.

- **API:** ASP.NET Core (`NumToWords.API`)
- **UI:** React + Vite (`num-to-words-ui`)

---

## Running with Docker Compose

Docker Compose builds and runs both services together. The UI is served by **nginx** and proxies API requests to the backend over the internal Docker network.

### Architecture

```
Browser  →  http://localhost:3000  (ui / nginx)
                ├─ GET  /              → React app (static files)
                └─ POST /convert       → api:8080 (ASP.NET Core, internal only)
```

The API is **not** published to the host. All browser traffic goes through the UI on port **3000**.

---

## Requirements

| Requirement | Notes |
|-------------|-------|
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine + Docker Compose v2 | `docker compose version` should work |
| ~2 GB free disk space | For image layers and build cache |
| Port **3000** available on the host | Used by the UI |

**Not required on the host:** .NET SDK, Node.js, or npm — everything is built inside Docker.

---

## Quick Start

From the `num_to_words` directory (where `docker-compose.yml` lives):

```bash
cd num_to_words
docker compose up --build
```

- `--build` rebuilds images when code or Dockerfiles change. Omit on later runs if nothing changed.
- Add `-d` to run in the background:

```bash
docker compose up --build -d
```

Open the app: **http://localhost:3000**

---

## Input Format

| Field | Format |
|-------|--------|
| Amount | `dollars,cents` — e.g. `1234,56` |
| Dollars | `0`–`999 999 999` (spaces allowed, stripped server-side) |
| Cents | `00`–`99` |
| Language | `english` or `german` (case-insensitive) |

---

## Design Decisions

### Architecture
The project follows **Vertical Slice Structure** , everything for the convert feature lives under `Features/Converter/` (endpoint, models, services, factory) instead of being split across separate layers like Controllers, Services, and Repositories. Adding another feature would mean adding another folder under `Features/`, not touching multiple layers.

### Currency Conversion
An abstract base class (`ConverterServiceBase`) holds shared logic: parsing the amount (`GetDollarsAndCents`), validation (`ValidateAmount`), digit grouping (`GetDigitsInHundredPos`), and logging. Each language subclass (`ConverterServiceEnglish`, `ConverterServiceGerman`) implements the language-specific pieces like word dictionaries, number-to-words conversion, and dollar/cent unit labels. A `ConverterServiceFactory` selects the right service based on the requested language. Both services are registered in DI in `Program.cs` and injected into the factory. Adding a third language means adding one new subclass, registering it in DI, and extending the factory. Existing language classes stay unchanged.

### Validation
Validation is split across three places:

- **Frontend (`ConvertForm.jsx`)** — regex and range checks before calling the API, with error messages from translations.
- **Endpoint (`ConverterEndpoint.cs`)** — basic checks for missing amount and supported language (`english` / `german`).
- **Service layer (`ConverterServiceBase.ValidateAmount` and each language service)** — digit checks and dollar/cent range limits; failures return `Results.BadRequest` via ASP.NET Core's `IResult`.

Services return `IResult` directly (success JSON or `BadRequest`), not a custom `ServiceResult<T>` wrapper. The endpoint stays relatively thin but still performs its own guard checks before delegating to the factory.

### Networking
In the Docker Compose setup, the API is not published to the host. It listens on port 8080 inside the Docker network only. The UI container serves the React app on port 3000 and nginx proxies `/convert` to the API container. The frontend is built with an empty `VITE_API_URL`, so requests go to `/convert` on the same origin and are forwarded internally. This mirrors a production setup where the API sits behind a reverse proxy.

### Language Support
UI text is centralised in `i18n/translation.jsx`, accessed via the `t(language, key)` helper. Switching language updates labels, help text, and error messages in the UI. The conversion output comes from the backend. The selected language is sent in the POST body and the factory returns the matching converter service. Adding a new language requires a new entry in `translation.jsx`, a new `ConverterService*` class on the backend, registration in `Program.cs`, and an update to `ConverterServiceFactory` (and the language dropdown in the form).

---

## Local Development (without Docker)

**API:**

```bash
cd NumToWords.API
dotnet run
```

**UI:**

```bash
cd num-to-words-ui
npm install
npm run dev
```

The UI defaults to `http://localhost:5000` for the API when `VITE_API_URL` is not set. Ensure CORS in `Program.cs` allows your Vite dev server origin (`http://localhost:5173`).

---

## Troubleshooting

### Port 3000 already in use

Another process is bound to port 3000.

```bash
lsof -i :3000
```

Either stop that process or change the host port in `docker-compose.yml`:

```yaml
ports:
  - "3001:80"   # use http://localhost:3001 instead
```

### `docker compose up` fails with build errors

1. Ensure you are in the `num_to_words` directory.
2. Rebuild without cache:

```bash
docker compose build --no-cache
docker compose up
```

3. Check API build output:

```bash
docker compose build api
```

### UI loads but conversion fails (network error in browser)

- Confirm both containers are running: `docker compose ps`
- Check API logs: `docker compose logs api`
- Test the proxy directly with the curl command in [Verifying the setup](#verifying-the-setup)
- If you changed `VITE_API_URL` at build time, rebuild the UI:

```bash
docker compose build ui
docker compose up -d
```

### macOS: port 5000 conflicts (local dev, not Docker)

On many Macs, **AirPlay Receiver** uses port 5000. This Docker setup avoids that by not exposing the API on 5000. If you run the API locally with `dotnet run`, you may see:

```
Failed to bind to address http://127.0.0.1:5000: address already in use
```

Options:

- Stop the other process: `lsof -i :5000`
- Disable AirPlay Receiver in **System Settings → General → AirDrop & Handoff**
- Change `applicationUrl` in `NumToWords.API/Properties/launchSettings.json`

### Stale containers after code changes

```bash
docker compose down
docker compose up --build
```

### View API errors in real time

```bash
docker compose logs -f api
```

Then trigger a request from the UI or `curl`.

### Cannot reach API directly on localhost:5000

This is expected. The API is only reachable inside the Docker network (`api:8080`). Use **http://localhost:3000/convert** from the host.

---

## Verifying the Setup

### UI loads

```bash
curl -I http://localhost:3000/
```

Expect `HTTP/1.1 200 OK`.

### Conversion via UI proxy

```bash
curl -X POST http://localhost:3000/convert \
  -H "Content-Type: application/json" \
  -d '{"amount":"123,45","language":"english"}'
```

Example response:

```json
{"amountInWords":"one hundred twenty three dollars and forty five cents"}
```

German example:

```bash
curl -X POST http://localhost:3000/convert \
  -H "Content-Type: application/json" \
  -d '{"amount":"323,00","language":"german"}'
```

---

## Project Layout

```
num_to_words/
├── docker-compose.yml
├── NumToWords.API/          # ASP.NET Core API
│   └── Dockerfile
└── num-to-words-ui/         # React + Vite frontend
    ├── Dockerfile
    └── nginx.conf
```

---

## AI Usage

I used **Cursor** (AI-assisted IDE) during development for learning, troubleshooting, manual testing, and documentation. I made the architectural decisions, implemented the code myself, and validated AI suggestions before keeping them. The codebase reflects deliberate design, not unchecked AI output.