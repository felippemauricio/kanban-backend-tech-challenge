# 📈 Kanban Board - BackEnd

![.NET 9](https://img.shields.io/badge/.NET-9C513B?style=for-the-badge&logo=dotnet&logoColor=white) ![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)

## 🏁 Getting Started

This project is a simple Kanban board application for managing tasks, built with .NET 9 for the backend.

It allows users to create, view, update, delete, and move tasks between columns (To Do → In Progress → Done). The application is designed to demonstrate clean architecture, maintainable code, and best practices.

## 📁 Project Structure

```
kanban-backend-tech-challenge/
├── .github/workflows/                           # GitHub Actions workflows for CI/CD
│   └── ci.yml                                   # Continuous Integration pipeline configuration
│
├── data/                                        # SQLite database files for development
│
├── src/                                         # Application source code
│   ├── KanbanBoard.Api/                         # Backend API project
│   │   ├── Controllers/                         # API controllers for handling HTTP requests
│   │   │   ├── BoardsController.cs              # CRUD operations for boards
│   │   │   ├── ColumnsController.cs             # CRUD operations for columns
│   │   │   └── TasksController.cs               # CRUD operations for tasks
│   │   ├── Program.cs                           # Application entry point
│   │   └── KanbanBoard.Api.csproj               # API project file
│   │
│   ├── KanbanBoard.Application/                 # Business logic layer
│   │   ├── Services/                            # Services that handle core application logic
│   │   │   ├── BoardService.cs                  # Logic related to boards
│   │   │   └── BoardTaskService.cs              # Logic related to tasks
│   │   └── KanbanBoard.Application.csproj       # Application layer project file
│   │
│   ├── KanbanBoard.Config/                      # Configuration files for the application
│   │   └── KanbanBoard.Config.csproj            # Application layer project file
│   │
│   ├── KanbanBoard.Domain/                      # Domain entities and models
│   │   └── KanbanBoard.Domain.csproj            # Domain layer project file
│   │
│   └── KanbanBoard.Infrastructure/              # Infrastructure layer
│       ├── Database/                            # Database context and SQLite integration
│       └── KanbanBoard.Infrastructure.csproj    # Infrastructure project file
│
├── tests/                                       # Unit and integration tests
│   ├── KanbanBoard.Api.Tests/                   # Tests for the API layer
│       └── KanbanBoard.Api.Tests.csproj         # Tests project file
│   └── KanbanBoard.Application.Tests/           # Tests for the application/services layer
│       └── KanbanBoard.Application.Tests.csproj # Tests project file
│
├── Dockerfile                                   # Dockerfile to containerize the backend
└── KanbanBoard.sln                              # .NET solution file containing all projects
```

## 🛠️ Tech Stack

- **.NET 9** – Backend API development
- **SQLite** – Lightweight database for storing boards and tasks
- **Docker** – Containerization for backend

## ⚙️ Installation

Follow these steps to run the Kanban Board application locally:

1. **Clone the repository**

```bash
git clone https://github.com/felippemauricio/kanban-backend-tech-challenge.git
cd kanban-backend-tech-challenge
```

2. Restore dependencies

```bash
dotnet restore
```

3. Run the application

```bash
cd src/KanbanBoard.Api
dotnet run
```

## ⚙️ Continuous Integration (CI)

This project has **CI configured with GitHub Actions**. On every push or pull request, the pipeline automatically runs:

- **Lint** – checks code formatting
- **Tests** – runs unit and integration tests
- **Docker build** – verifies that the backend Docker image builds correctly

### Commands run in CI

- **Lint**

```bash
dotnet format --verify-no-changes
```

- **Tests**

```bash
dotnet test --no-restore --verbosity normal
```

## 🚀 Deployment

The Kanban Board backend is deployed on **Render** and can be accessed via:

[https://kanban-backend-fd4x.onrender.com](https://kanban-backend-fd4x.onrender.com)

You can explore the API using **Swagger** at the same URL.

> ⚠️ Note: This project is running on a free Render account, so the first request may take a few seconds to start the server. If the page doesn’t load immediately, please refresh after a moment.

## 💡 Curiosities

- **Idempotent Endpoints** – All API endpoints are idempotent. Even task creation supports idempotency via an `idempotency-key` header to prevent duplicate operations if the request is retried.

- **LexoRank** – I learned and implemented a technique called **LexoRank** to manage task ordering. This allows users to move tasks between columns or reorder them without having to update the position of every task in the database. It’s efficient and scalable for a Kanban board application.
