# LuxeCart

E-commerce web application: an Angular 17 single-page app, an ASP.NET Core Web API and a SQL Server database,
built as the semester project for *Instrumente pentru dezvoltarea programelor*.

The project builds on the author's earlier work and is being published and extended step by step for the course, with the professor's approval.

> Status: in development. The specification is in [`docs/`](docs/).

## Planned stack

| Layer     | Technology                                           |
|-----------|------------------------------------------------------|
| Frontend  | Angular 17, TypeScript, RxJS                         |
| Backend   | ASP.NET Core Web API, ADO.NET (no ORM)               |
| Database  | Microsoft SQL Server 2022                            |
| Security  | JWT Bearer, BCrypt, login rate limiting              |
| Tests     | xUnit, Moq (backend) / Jasmine, Karma (frontend)     |
| Delivery  | Docker Compose, GitHub Actions                       |

## Repository layout

```
backend/    ASP.NET Core API and its tests
frontend/   Angular application
database/   SQL schema and seed data
docs/       specification and diagrams
```

## Working agreement

- Planning is done in Jira (project key `LUX`); every change starts from a Jira issue.
- Branches: `feature/LUX-<n>-<short-name>`; work is merged into `main` through a pull request.
- Commit messages start with the issue key: `LUX-<n> <type>: <summary>` (`feat`, `fix`, `docs`, `test`, `ci`, `build`, `chore`).
- Each report milestone is marked with a tag: `raport-1`, `raport-2`, `raport-final`.
