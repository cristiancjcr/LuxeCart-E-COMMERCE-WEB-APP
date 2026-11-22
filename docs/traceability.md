# Traceability

Every change in this repository starts from a Jira issue (project key `LUX`). The issue key appears in the branch name,
in each commit message and in the pull request title, so any change can be traced both ways:

- from a Jira issue to its pull request: search the repository's pull requests for the key (for example `LUX-23`);
- from a commit or pull request back to the issue: read the key at the start of the title.

The full Jira export is in [`jira/`](jira/).

## Issues and sprints

| Sprint | Issue  | Subject |
|--------|--------|---------|
| 1 | LUX-6  | Report 1: specification |
| 1 | LUX-7  | Report 1: use case diagram and technologies |
| 1 | LUX-8  | Database schema and seed data |
| 2 | LUX-9  | API foundation: models, DTOs, database access, health checks |
| 2 | LUX-10 | Authentication API (register, login, JWT) |
| 2 | LUX-11 | Authentication unit tests |
| 2 | LUX-12 | Products API |
| 3 | LUX-13 | Angular workspace and bootstrap |
| 3 | LUX-14 | Frontend core: models, toast, authentication, interceptors, cart service |
| 3 | LUX-15 | Application shell and authentication screens |
| 3 | LUX-16 | Product catalog |
| 4 | LUX-17 | Backend Dockerfile |
| 4 | LUX-18 | Frontend Dockerfile and nginx |
| 4 | LUX-19 | Docker Compose |
| 4 | LUX-20 | Bug: production build budgets and API URL |
| 4 | LUX-21 | Bug: .NET version mismatch in Docker and CI (net8.0) |
| 4 | LUX-22 | Bug: failing frontend unit tests |
| 4 | LUX-23 | CI pipeline with GitHub Actions |
| 5 | LUX-24 | Cart and order services |
| 5 | LUX-25 | Cart and order endpoints |
| 5 | LUX-26 | Order service unit tests |
| 5 | LUX-27 | Cart page |
| 5 | LUX-28 | Checkout and order history pages |
| 5 | LUX-29 | Admin API |
| 5 | LUX-30 | Admin dashboard |
| 5 | LUX-31 | Final documentation and Jira export |

LUX-1 to LUX-5 are the epics that group these issues.
