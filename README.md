# Crud-Services

A lightweight and scalable ASP.NET Core microservice designed for performing CRUD (Create, Read, Update, Delete) operations. This project complements the [Auth Service](https://github.com/sivavithu/Authenthication-Service-Basic) by providing secure, RESTful endpoints for managing data with JWT-based authorization. Built with beginners in mind, it offers a clear code structure, detailed setup instructions, and integration with Entity Framework Core for flexible database interactions (e.g., SQL Server, PostgreSQL). Ideal for developers learning microservices, REST API development, or building modular systems that integrate with API gateways like Ocelot.

## ✨ Why This CRUD Service?
This microservice is crafted to simplify data management in a microservices architecture while maintaining security and scalability. Key highlights include:

- **Beginner-Friendly**: Clean, well-commented code and step-by-step setup make it accessible for those new to .NET or microservices.
- **Secure Integration**: Leverages JWT tokens from the Auth Service for role-based authorization, ensuring only authenticated users access endpoints.
- **Flexible Database Support**: Uses Entity Framework Core for seamless integration with relational databases like SQL Server
- **RESTful Design**: Follows REST principles with intuitive endpoints for creating, reading, updating, and deleting resources.
- **Scalable and Modular**: Designed to work with API gateways and other microservices (e.g., notification or analytics services) in a larger ecosystem.
- **Extensible**: Easily add custom entities, endpoints, or advanced features like caching or bulk operations.

## 🚀 Features
The Crud-Services microservice provides robust functionality for data management:

- **Create**: Add new records with POST endpoints, ensuring data validation and unique constraints.
- **Read**: Retrieve single or multiple records via GET endpoints with support for filtering and pagination.
- **Update**: Modify existing records with PUT or PATCH endpoints, maintaining data integrity.
- **Delete**: Remove records securely with DELETE endpoints, restricted by user roles.
- **JWT Authorization**: Validates tokens from the Auth Service, enforcing role-based access (e.g., "Admin" for delete operations).
- **Database Integration**: Uses EF Core for efficient, asynchronous data operations with support for migrations.
- **Error Handling**: Returns clear HTTP status codes (e.g., 404 for not found, 403 for unauthorized) and descriptive messages.
- **Extensible Models**: Supports custom entities (e.g., products, users) with minimal code changes.

## 🛠️ Prerequisites
Ensure you have the following tools installed:

- **.NET SDK** (version 8 or later) – download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
- **SQL Server** (Express edition for development) – install from Microsoft or use a cloud instance like Azure SQL. Other relational DBs are supported via EF Core.
- **Visual Studio 2022** or VS Code with C# extension for editing, debugging, and migrations.
- **Postman** or curl for testing API endpoints.
- Optional: Docker for containerized deployment to cloud platforms like Azure or AWS.
- **Auth Service**: Running instance of the [Auth Service](https://github.com/sivavithu/Authenthication-Service-Basic) for JWT token generation.

## 📋 Installation and Setup
Follow these steps to set up and run the Crud-Services microservice locally:

1. **Clone the Repository**
   ```bash
   git clone https://github.com/sivavithu/Crud-Services.git
   cd Crud-Services
   ```

2. **Restore Dependencies**  
   Install required NuGet packages:  
   ```bash
   dotnet restore
   ```

3. **Configure Secrets Securely**  
   
   **IMPORTANT**: Never commit secrets to source control. This project uses .NET User Secrets for development and supports environment variables.

   **Option A: Using .NET User Secrets (Recommended for Development)**
   
   Initialize and set your secrets using the following commands:
   ```bash
   cd CrudServices
   
   # Set JWT Configuration
   dotnet user-secrets set "AppSettings:issuer" "loginapp"
   dotnet user-secrets set "AppSettings:Audience" "myAwesomeAudience"
   dotnet user-secrets set "AppSettings:Key" "your-long-secure-key-here-at-least-32-characters"
   
   # Set Database Connection String
   dotnet user-secrets set "ConnectionStrings:BookDatabase" "Server=your-server-name;Database=BookDb;Trusted_Connection=true;TrustServerCertificate=true;"
   ```
   
   To view your configured secrets:
   ```bash
   dotnet user-secrets list
   ```

   **Option B: Using Environment Variables**
   
   Copy `.env.example` to `.env` and update with your values:
   ```bash
   cp .env.example .env
   ```
   
   Then edit `.env` with your actual configuration values. Note: You'll need to load these environment variables before running the application.

   **Option C: Using appsettings.Development.json (Local Only)**
   
   Copy the example file and update with your values:
   ```bash
   cp CrudServices/appsettings.Development.json.example CrudServices/appsettings.Development.json
   ```
   
   Then edit `appsettings.Development.json` with your actual configuration. This file is gitignored and will not be committed.
   
   **For Production**: Use Azure Key Vault, AWS Secrets Manager, or your cloud provider's secrets management service.

4. **Apply Database Migrations**  
   Create the initial database schema:  
   ```bash
   dotnet ef migrations add InitialCreate
   ```  
   Apply the migration:  
   ```bash
   dotnet ef database update
   ```  
   This creates the "CrudDb" database with tables for your entities (e.g., Products, Categories). Update the EF Core provider in `Program.cs` for non-SQL Server databases.

5. **Run the Service**  
   Start the application:  
   ```bash
   dotnet run
   ```  
   Or use Visual Studio: Press F5 to debug or Ctrl+F5 to run without debugging.  
   The service will listen on `http://localhost:5002` (or HTTPS if configured). Look for "Application started. Press Ctrl+C to shut down" in the console.

6. **Test the Endpoints**  
   Use Postman or curl with a valid JWT token from the Auth Service:  
   - **Create**: POST `http://localhost:5002/api/resources` with `Authorization: Bearer <jwt-token>` and JSON body (e.g., `{"name": "Product1", "description": "Test"}`) – expect 201 Created.  
   - **Read**: GET `http://localhost:5002/api/resources` – expect 200 OK with a list of resources.  
   - **Update**: PUT `http://localhost:5002/api/resources/{id}` with updated JSON body – expect 200 OK or 404 Not Found.  
   - **Delete**: DELETE `http://localhost:5002/api/resources/{id}` – expect 204 No Content (or 403 if unauthorized).  
   Check console logs for errors and verify the connection string and token validity.

## 📂 Project Structure
```plaintext
Crud-Services/
├── CrudService/                 # Main service project directory
│   ├── Controllers/             # API Controllers (e.g., ResourceController.cs)
│   ├── Models/                  # Entity models and DTOs (e.g., Resource.cs, ResourceDto.cs)
│   ├── Services/                # Business logic (e.g., IResourceService.cs, ResourceService.cs)
│   ├── Data/                    # EF Core DbContext and migrations
│   ├── Program.cs               # Entry point, service configuration
│   ├── appsettings.json         # Application config (DB, JWT settings)
│   └── ...                      # Other .NET project files
├── Dockerfile                   # Containerization support
├── README.md                    # Project info (you're here!)
├── .gitignore                   # Ignored files
└── CrudService.sln              # Visual Studio solution file
```

## 🔧 Configuration Details
- **JWT Authorization**: Configured in `Program.cs` with `AddAuthentication` to validate tokens from the Auth Service, ensuring secure endpoint access.
- **Database Access**: Uses EF Core for asynchronous CRUD operations with change tracking and migrations.
- **Error Handling**: Implements global exception handling for consistent error responses (e.g., 400 for invalid input, 500 for server errors).
- **Logging**: Basic console logging included; extend with Serilog for advanced logging in production.

## 🤝 Contributing
This project is open to contributions! To add features like caching, advanced filtering, or new entity types, fork the repo and submit a pull request. Report issues or suggest improvements via GitHub Issues to enhance the community experience.

## 📄 License
MIT License – Free to use, modify, and distribute. See the LICENSE file for details.

## 🌟 Star This Repo
If this project helps you learn or build, please give it a star on GitHub! Explore the [Auth Service](https://github.com/sivavithu/Authenthication-Service-Basic) and API Gateway repos for a complete microservices stack.
