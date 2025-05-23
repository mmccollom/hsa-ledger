# HSA Ledger

**HSA Ledger** is a web-based application for managing Health Savings Account (HSA) transactions. It enables users to track expenses, associate them with individuals, providers, and transaction types, and monitor HSA withdrawals and payments. Built on **.NET 8** and **Blazor WebAssembly**, the project follows Clean Architecture principles for maintainability and scalability.

---

## ✨ Features

- ✅ **Blazor WebAssembly** client for a modern, responsive UI  
- ✅ **ASP.NET Core Web API** using .NET 8  
- ✅ **Entity Framework Core** with PostgreSQL for persistence  
- ✅ **CQRS with MediatR** for clear separation of reads/writes  
- ✅ **FluentValidation** for business rule enforcement  
- ✅ **AWS Lambda Integration** to extract SES email attachments  
- ✅ **Modular Clean Architecture** with shared abstractions and infrastructure  

---

## 🧱 Project Structure

```
src/
├── Application/             # CQRS handlers, validation, business rules
├── Client/                  # Blazor WebAssembly frontend
├── Client.Infrastructure/   # Http clients and services for Blazor client
├── Domain/                  # Domain models, enums, and interfaces
├── Server/                  # ASP.NET Core Web API (with Dockerfile)
├── Server.Infrastructure/   # EF Core + Auth/Storage integrations
├── Lambda.EmailReader/      # AWS Lambda for processing SES email attachments
├── Lambda.Infrastructure/   # Shared Lambda service dependencies
├── Shared/                  # Shared contracts and utilities
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- PostgreSQL (local or cloud)

### API (Server)

```bash
cd src/Server
dotnet run
```

You can configure your environment using `appsettings.Development.json` or user secrets.

### Client (Blazor WebAssembly)

```bash
cd src/Client
dotnet run
```

The client interacts with the API and supports HSA-related workflows out of the box.

---

## 🌐 Deployment

- **Client** is a Blazor WebAssembly app designed for static hosting (e.g., AWS S3 + CloudFront):

```bash
dotnet publish src/Client/Client.csproj -c Release -o publish
```

- **API (Server)** includes a `Dockerfile` (in `src/Server`) to support container-based deployments.

- **Lambda.EmailReader** is an AWS Lambda function that:
  - Triggers from SES email receipt
  - Extracts PDF attachments
  - Posts to the main Web API

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
