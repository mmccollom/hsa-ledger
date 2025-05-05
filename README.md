# HSA Ledger

**HSA Ledger** is a web-based HSA ledger built with **Blazor WebAssembly**, **.NET**, and **PostgreSQL**. It follows a **Clean Architecture** structure, applying modern development practices like **CQRS with MediatR**, **FluentValidation**, and containerized development using **Docker**.

---

## 🚀 Features

- Blazor WebAssembly frontend
- ASP.NET Core Web API backend
- PostgreSQL database with Entity Framework Core
- Clean Architecture:
    - Domain
    - Application
    - Infrastructure
    - UI (Blazor WASM)
- CQRS via MediatR
- Validation using FluentValidation
- Containerized development environment with Docker Compose

---

## 📁 Project Structure

```
hsa-ledger/
│
├── src/                                # All source code
│   ├── Domain/          
│   ├── Application/     
│   ├── Client.Infrastructure/
│   ├── Server.Infrastructure/  
│   └── Client/  
│   └── Server/         
│   └── Shared/                         # Shared between Client / Server / Infrastructure              
│
├── docker/                             # Docker-related files
│   ├── docker-compose.yml              # Docker Compose setup
│   └── .env                            # Environment variables (not in source)
│
└── README.md
```

---

## 🧪 Tech Stack

- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **Architecture**: Clean Architecture
- **Patterns**: CQRS (via MediatR)
- **Validation**: FluentValidation
- **Containerization**: Docker & Docker Compose

---

## 🛠️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- [Docker](https://www.docker.com/products/docker-desktop)

### Environment Setup

1. Create a `.env` file in the `/docker` directory:

```ini
AWS_ACCESS_KEY_ID=your_aws_access_key_id
AWS_SECRET_ACCESS_KEY=your_aws_secret_access_key
AWS_REGION=your_aws_region
ConnectionStrings__DefaultConnection=Host=postgres;Port=5433;Database=hsa_ledger;Username=hsa;Password=dev_testing!
```

> ⚠️ Do **not** commit this file to version control. It contains sensitive credentials.

### Running the App Locally

1. Clone the repository:

```bash
git clone https://github.com/mmccollom/hsa-ledger.git
cd hsa-ledger
```

2. Build and start containers from the `/docker` directory:

```bash
cd docker
docker-compose up --build
```

This will spin up:
- The Web API (default: `http://localhost:8081`)
- A Postgres container with the configured user and database

3. (Optional) Run the Blazor UI locally (if not containerized):

```bash
dotnet run --project ../src/Client
```

---

## ⚙️ Configuration

The API uses `appsettings.json` and environment variables for configuration. Connection strings and credentials should be provided via the `.env` file and passed through the `docker-compose.yml`.

Example `appsettings.Development.json` snippet:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=hsa_ledger;Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"
}
```

---

## 📦 Deployment

This application can be built and deployed using Docker for consistent environments across dev, staging, and production. You can extend the `docker-compose.yml` with production settings or use container orchestrators like **Kubernetes** or **Docker Swarm**.

---

## 📄 License

[MIT](LICENSE)

---

**Developed with ❤️ using Blazor and .NET**
