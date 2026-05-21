# 🏢 Sales ERP System

A full-featured **Enterprise Resource Planning (ERP)** web application for managing sales operations — built with **ASP.NET Core MVC (.NET 9)** following clean **Onion Architecture** and industry-standard design patterns.

---

## 🛠️ Technologies & Concepts Used

### ⚙️ Backend & Framework
- **ASP.NET Core MVC (.NET 9)** — Web framework with MVC pattern
- **C#** — Primary programming language
- **Entity Framework Core 9** — ORM for database operations (Code-First)
- **Microsoft SQL Server** — Relational database

### 🔐 Identity, Authentication & Authorization
- **ASP.NET Core Identity** — User management, password hashing, sign-in/sign-out
- **Role-Based Authorization** — Three roles with scoped permissions:
  | Role | Access Level |
  |---|---|
  | `Admin` | Full access — user management, payments, all modules |
  | `SalesEmployee` | Sales operations — orders, customers |
  | `WarehouseEmployee` | Inventory & purchase operations |
- **`[Authorize]` attribute** — Declarative controller & action-level access control
- **Cookie-based Authentication** — Session management via Identity middleware

### 🧱 Architecture — Onion Architecture (N-Tier)
The project is structured in 4 independent layers, each depending only inward:

```
┌─────────────────────────────────┐
│        ERP.App (MVC)            │  ← Presentation Layer
├─────────────────────────────────┤
│      ERP.Services               │  ← Business Logic Layer
├─────────────────────────────────┤
│      ERP.Repositories           │  ← Data Access Layer
├─────────────────────────────────┤
│      ERP.Domain                 │  ← Core Domain (Models & Enums)
└─────────────────────────────────┘
```

### 🎯 Design Patterns
- **Repository Pattern** — `IGenericRepository<T>` abstracts all data access logic
- **Unit of Work Pattern** — `IUnitOfWork` manages transactions across multiple repositories
- **Service Layer Pattern** — Business logic isolated in dedicated service classes, separate from controllers

### 🔩 SOLID Principles
- **S** — Single Responsibility: each class has one job (controller → handles HTTP, service → business logic, repository → data access)
- **O** — Open/Closed: generic repository is extensible without modification
- **L** — Liskov Substitution: services and repositories implement interfaces and are fully interchangeable
- **I** — Interface Segregation: separate interfaces per service (`ICategoryService`, `IOrderService`, etc.)
- **D** — Dependency Inversion: all dependencies injected via interfaces, not concrete classes

### 💉 Dependency Injection
- All services, repositories, and the Unit of Work are registered using **ASP.NET Core's built-in DI container**
- `Scoped` lifetime used for all services and repositories, ensuring per-request isolation

### 🖼️ Frontend
- **Razor Views (`.cshtml`)** — Server-side rendering with strongly-typed ViewModels
- **Bootstrap 5.3** — Responsive UI components and layout grid
- **Bootstrap Icons 1.11** — Icon library
- **ViewModels** — Dedicated ViewModel classes for each view, keeping domain models clean

---

[GitHub](https://github.com/abdallaKD)

