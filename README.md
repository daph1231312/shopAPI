# ShopAPI — E-commerce RESTful Web Service

C# ASP.NET Core 9 + React · Richardson Maturity Level 4 · SQLite · xUnit tests · Swagger docs

---

## Quick Start (5 minutes)

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)


The SQLite database (`shop.db`) is created automatically with seed data on first run.


---

## API Endpoints

### Products
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | List all (filter: `?categoryId=1`) |
| GET | `/api/products/{id}` | Get by ID |
| POST | `/api/products` | Create |
| PUT | `/api/products/{id}` | Full update |
| DELETE | `/api/products/{id}` | Delete |

### Categories
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | List all |
| GET | `/api/categories/{id}` | Get by ID |
| POST | `/api/categories` | Create |
| DELETE | `/api/categories/{id}` | Delete (fails if has products) |

### Orders
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | List all |
| GET | `/api/orders/{id}` | Get by ID |
| POST | `/api/orders` | Place order (validates stock) |
| PATCH | `/api/orders/{id}/status` | Update status only |

---

## HATEOAS (Level 4 Richardson)

Every response includes a `_links` field (this is the hypermedia that qualifies as Level 4):

```json
{
  "id": 1,
  "name": "Wireless Headphones",
  "price": 79.99,
  "links": {
    "self":   "http://localhost:5000/api/products/1",
    "update": "http://localhost:5000/api/products/1",
    "delete": "http://localhost:5000/api/products/1",
    "all":    "http://localhost:5000/api/products"
  }
}
```

---

## SOLID Principles Applied

| Principle | Where |
|-----------|-------|
| **S** — Single Responsibility | Each class has one job: Controllers handle HTTP, Repos handle DB, LinkService handles links |
| **O** — Open/Closed | Add new repo implementations without changing controllers |
| **L** — Liskov Substitution | `ProductRepository` can be replaced by any `IProductRepository` impl |
| **I** — Interface Segregation | Separate `IProductRepository`, `ICategoryRepository`, `IOrderRepository` |
| **D** — Dependency Inversion | Controllers depend on interfaces, not concrete classes |

---

## Caching (Stateless requirement)

GET `/api/products` and `/api/categories` use `[ResponseCache(Duration=60)]` — the API is stateless (no session), caching is done via HTTP headers.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | C# ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQLite (file: `shop.db`) |
| Documentation | Swagger / OpenAPI (Swashbuckle) |
| Unit Tests | xUnit + FluentAssertions + Moq |
| Acceptance Tests | Postman Collection (Newman compatible) |
| Frontend | React 18 + Vite |

---

## Team Roles (for assignment)

| Role | Responsibility |
|------|---------------|
| Developer | Backend API, EF Core models, HATEOAS |
| QA | xUnit tests, Postman collection |
| Analyst | API design, Swagger documentation review |

---

## GitHub Workflow

```bash
git init
git checkout -b develop
# work on feature branch
git checkout -b feature/products-controller
git add .
git commit -m "feat: add ProductsController with CRUD"
git checkout develop
git merge feature/products-controller
# when ready for submission
git checkout main
git merge develop
```
