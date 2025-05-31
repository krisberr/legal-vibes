# Legal Vibes

Legal Vibes is an AI-powered web portal designed to help IP lawyers create and draft legal applications for trademarks and other intellectual property documents.

## Project Structure

```
LegalVibes/
├── src/
│   ├── LegalVibes.API/          # API endpoints, controllers
│   ├── LegalVibes.Application/  # Application logic, interfaces
│   ├── LegalVibes.Domain/       # Domain entities, business rules
│   └── LegalVibes.Infrastructure/ # External concerns (DB, external services)
└── client/                      # React frontend
```

## Technology Stack

### Backend
- .NET 9.0
- Entity Framework Core
- OpenAI API Integration
- JWT Authentication
- Serilog for logging
- Swagger/OpenAPI

### Frontend
- React with TypeScript
- Vite
- Modern UI components
- Secure authentication
- Real-time updates

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Node.js (Latest LTS)
- SQL Server (or your preferred database)
- OpenAI API Key (for AI features)

### Backend Setup
1. Navigate to the API project:
   ```bash
   cd src/LegalVibes.API
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Update database:
   ```bash
   dotnet ef database update
   ```
4. Run the API:
   ```bash
   dotnet run
   ```

### Frontend Setup
1. Navigate to the client directory:
   ```bash
   cd client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm run dev
   ```

## Features

- User Authentication & Authorization
- Project Management
- AI-Powered Document Generation
- Document Management
- Client Information Management
- Trademark Application Processing

## Development

### API Endpoints
- Swagger UI: https://localhost:7032/swagger
- Health Check: https://localhost:7032/api/healthcheck

### Environment Variables
Create a `.env` file in the client directory with:
```env
VITE_API_URL=https://localhost:7032
```

## Architecture

The solution follows Clean Architecture principles:

1. **Domain Layer**: Core business logic and entities
2. **Application Layer**: Use cases and business logic orchestration
3. **Infrastructure Layer**: External concerns implementation
4. **API Layer**: REST API endpoints and controllers
5. **Client**: React frontend application

## Contributing

1. Create a feature branch
2. Commit your changes
3. Push to the branch
4. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details 