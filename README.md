# Cafe Management Web Application

A comprehensive cafe management system built with .NET 8, Entity Framework Core, and SQL Server LocalDB. This project demonstrates modern web development practices and serves as an excellent portfolio piece for students.

## Project Overview

This application provides a complete cafe management solution with:
- **Customer Interface**: QR code-based ordering, menu browsing, cart management
- **Admin Interface**: Product management, order tracking, dashboard analytics
- **Authentication**: Role-based access control (Admin, Manager, Staff, Customer)
- **Database**: SQL Server LocalDB with Entity Framework Core


## Technology Stack

- **Backend**: .NET 8 Web MVC + Web API
- **Database**: SQL Server LocalDB
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **Session Management**: In-Memory caching
- **Architecture**: Clean Architecture with Repository Pattern

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code
- SQL Server LocalDB (included with Visual Studio)

## Quick Start

1. **Clone and Setup**
   ```bash
   git clone <repository-url>
   cd CafeApp
   ```

2. **Restore Packages**
   ```bash
   dotnet restore
   ```

3. **Run the Application**
   ```bash
   dotnet run --project CafeApp.Web
   ```

4. **Access the Application**
   - **Customer Interface**: http://localhost:5095
   - **Admin Interface**: http://localhost:5095/Admin
   - **Login**: admin@cafeapp.com / Admin123!

## Project Structure

```
CafeApp/
├── CafeApp.Core/              # Domain entities and interfaces
│   ├── Entities/              # Domain models
│   ├── Enums/                 # Enumerations
│   └── Interfaces/            # Service contracts
├── CafeApp.Data/              # Data access layer
│   ├── Repositories/          # Repository implementations
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── CafeApp.Services/          # Business logic layer
├── CafeApp.Web/               # Presentation layer
│   ├── Areas/Admin/           # Admin management interface
│   ├── Controllers/           # Customer-facing controllers
│   └── Views/                 # Razor views (to be created)
└── CafeApp.Tests/             # Unit tests
```

## Default Credentials

- **Admin User**: admin@cafeapp.com
- **Password**: Admin123!
- **Roles**: Admin, Manager, Staff, Customer

## Sample Data

The application comes pre-loaded with:
- **6 Categories**: Coffee, Tea, Pastries, Sandwiches, Desserts, Smoothies
- **26 Products**: Complete menu with pricing
- **1 Sample Order**: For testing order management
- **4 User Roles**: Proper role-based access control


## Architecture Highlights

- **Clean Architecture**: Separation of concerns with distinct layers
- **Repository Pattern**: Abstracted data access
- **Dependency Injection**: Proper IoC container usage
- **Entity Framework**: Code-first approach with migrations
- **Identity Framework**: Secure authentication and authorization

## Portfolio Value

This project demonstrates:
- **Full-Stack Development**: Complete web application
- **Modern .NET Practices**: Latest .NET 8 features
- **Database Design**: Proper relational database structure
- **Security**: Authentication and authorization
- **Clean Code**: SOLID principles and best 
