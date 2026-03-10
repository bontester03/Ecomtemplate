# Khan Home Floral Line - Monorepo

Production-ready starter for a UAE flower shop e-commerce platform.

## Stack
- Backend: ASP.NET Core Web API (.NET 8), EF Core, PostgreSQL
- Frontend: Angular 19 (standalone), TypeScript, Angular Material
- Auth: JWT + refresh tokens, role-based access (Admin/Staff/Customer)
- Storage: Blob storage abstraction + local dev implementation
- Payments: gateway abstraction + `MockGateway`

## Monorepo Layout
- `api/` - .NET API + clean layers
- `web/` - Angular storefront + admin GUI
- `infra/` - Docker compose and local infrastructure

## Backend Architecture
- `KhanHomeFloralLine.Domain` - entities/enums
- `KhanHomeFloralLine.Application` - DTOs/service contracts/pricing logic
- `KhanHomeFloralLine.Infrastructure` - EF Core, auth, payment, storage implementations
- `KhanHomeFloralLine.Api` - controllers, middleware, auth, swagger
- `KhanHomeFloralLine.Tests` - unit tests for promo/delivery/pricing

## Local Setup
### Prerequisites
- .NET SDK 8+
- Node 22+
- Docker (optional)
- PostgreSQL 15+ (if not using Docker)

### API
1. Update connection + JWT settings in `api/KhanHomeFloralLine.Api/appsettings.Development.json`.
2. Restore/build:
   - `dotnet restore KhanHomeFloralLine.slnx`
   - `dotnet build api/KhanHomeFloralLine.Api/KhanHomeFloralLine.Api.csproj`
3. Run API:
   - `dotnet run --project api/KhanHomeFloralLine.Api`
4. Swagger:
   - `http://localhost:5000/swagger` (or configured launch URL)

### EF Migrations
- Initial migration is generated under `api/KhanHomeFloralLine.Infrastructure/Persistence/Migrations`.
- Apply manually if needed:
  - `dotnet ef database update --project api/KhanHomeFloralLine.Infrastructure --startup-project api/KhanHomeFloralLine.Api`

### Frontend
1. Install deps:
   - `cd web && npm ci`
2. Run:
   - `npm start`
3. Frontend URL:
   - `http://localhost:4200`

## Seed Data
Seeded in `AppDbContext`:
- Admin: `admin@khanhomefloral.ae`
- Sample categories/products/variants/add-ons
- Delivery zones + slots
- Promo code: `WELCOME10`
- Setting: `SameDayCutoffHour = 14`

## Environment Variables
### API
- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__AccessTokenMinutes`
- `Storage__AzureBlobConnectionString`
- `Storage__ContainerName`
- `Payment__Gateway`
- `Payment__Telr__StoreId`
- `Payment__PayTabs__ProfileId`
- `Payment__Stripe__SecretKey`

### Frontend
- Set API URL in `web/src/app/core/models/app-config.ts` (`apiBaseUrl`)

## Docker (Local)
From `infra/`:
- `docker compose up --build`

Services:
- API: `http://localhost:5000`
- Web: `http://localhost:4200`
- PostgreSQL: `localhost:5432`

## Azure Deployment Notes (Linux)
1. Create Azure Database for PostgreSQL Flexible Server.
2. Create Azure Storage account + Blob container `product-images`.
3. Create Azure App Service (Linux) for API container.
4. Create Web App (Linux) or Static Web App for Angular output.
5. Configure API app settings:
   - PostgreSQL connection string
   - JWT secrets
   - Blob connection string/container
   - CORS allowed origin to frontend domain
6. Enable HTTPS only in App Service.
7. Configure health checks and App Insights logs.

## Testing
Run backend unit tests:
- `dotnet test api/KhanHomeFloralLine.Tests/KhanHomeFloralLine.Tests.csproj`

## Notes
- Payment webhooks for Telr/PayTabs/Stripe are intentionally placeholder endpoints.
- Checkout totals are computed server-side.
- Guest cart is frontend-local; logged-in cart persistence is available via `/api/cart`.

