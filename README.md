# Student Management System

A full-stack Student Management System built with ASP.NET Core Web API and React.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete students
- **JWT Authentication**: Secure API endpoints with JWT tokens
- **Global Exception Handling**: Middleware for centralized error handling
- **Logging**: Serilog integration for file and console logging
- **Swagger API Documentation**: Interactive API documentation
- **Layered Architecture**: Controller, Service, Repository pattern
- **React UI**: Modern responsive frontend with TypeScript

## Technology Stack

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Serilog
- Swagger

### Frontend
- React 18
- TypeScript
- Axios
- React Router

## Project Structure

```
StudentManagementApi/           # ASP.NET Core Web API
├── Controller/                 # API Controllers
├── Service/                    # Business Logic
├── Repository/                 # Data Access
├── Models/                     # Entity Models
├── DTOs/                       # Data Transfer Objects
├── Data/                       # Database Context
├── Middleware/                 # Custom Middleware
└── appsettings.json            # Configuration

student-management-ui/           # React Frontend
├── src/
│   ├── components/             # React Components
│   ├── services/               # API Services
│   └── types/                  # TypeScript Types
└── package.json
```

## Prerequisites

- .NET 8.0 SDK
- SQL Server
- Node.js 18+
- npm or yarn

## Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Execute the SQL script in `StudentManagementApi/DatabaseSetup.sql`
3. The script creates:
   - Database: `StudentDB`
   - Table: `Students`
   - Sample data (optional)

## API Setup

1. Navigate to the API project directory:
   ```bash
   cd StudentManagementApi
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=StudentDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

5. The API will be available at:
   - Swagger UI: `http://localhost:5000/swagger`
   - API: `http://localhost:5000/api`

## UI Setup

1. Navigate to the UI project directory:
   ```bash
   cd student-management-ui
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

4. Open your browser and navigate to:
   - `http://localhost:3000`

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | Login and get JWT token |

### Students (Requires JWT)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/students | Get all students |
| GET | /api/students/{id} | Get student by ID |
| POST | /api/students | Create new student |
| PUT | /api/students | Update student |
| DELETE | /api/students/{id} | Delete student |

## Demo Credentials

- **Username**: admin
- **Password**: admin123

## Configuration

### JWT Settings (appsettings.json)
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere12345678901234567890",
    "Issuer": "StudentManagementApi",
    "Audience": "StudentManagementApi",
    "ExpiryMinutes": 60
  }
}
```

### API Base URL
Update `student-management-ui/src/services/api.ts` if your API runs on a different port:
```typescript
const API_BASE_URL = 'http://localhost:5000/api';
```

## Building for Production

### API
```bash
cd StudentManagementApi
dotnet publish -c Release
```

### UI
```bash
cd student-management-ui
npm run build
```

## Architecture

The application follows a layered architecture:

1. **Controller Layer**: Handles HTTP requests/responses
2. **Service Layer**: Business logic and validation
3. **Repository Layer**: Data access operations
4. **Model Layer**: Database entities

## Error Handling

- Global Exception Middleware handles all unhandled exceptions
- Structured error responses with status codes
- Serilog logging for all operations

## Security

- JWT Bearer authentication for protected endpoints
- Password validation
- Input validation on all DTOs
- SQL injection prevention via Entity Framework

## License

This project is for educational purposes.
