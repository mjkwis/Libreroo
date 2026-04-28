# Libreroo

Demo MVP with:
- API: .NET (`apps/libreroo-api`)
- Web: Angular (`apps/libreroo-web`)
- DB: PostgreSQL (Docker)

## Local Run (MVP)

1. Start PostgreSQL:
```bash
docker compose up -d
```

2. Start API:
```bash
dotnet run --project apps/libreroo-api
```

3. Start Web:
```bash
cd apps/libreroo-web
npm install
npm start
```

4. Open:
```text
http://localhost:4200
```

Notes:
- API runs on `http://localhost:5141`.
- On startup, API applies EF Core migrations automatically in non-testing environments.
