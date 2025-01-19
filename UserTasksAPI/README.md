# User Tasks API

## Overview
The User Tasks API is a RESTful web service built with ASP.NET Core that allows users to manage their tasks. It provides endpoints for user and task management, including CRUD operations and authentication.

## Features
- User management (create, read, update, delete)
- Task management (create, read, update, delete)
- Authentication using API keys or JWT tokens
- Filtering tasks based on due dates
- Swagger documentation for API endpoints

## Project Structure
```
UserTasksAPI
├── Controllers
│   ├── UsersController.cs
│   └── TasksController.cs
├── Models
│   ├── User.cs
│   └── Task.cs
├── Repositories
│   ├── IUserRepository.cs
│   ├── ITaskRepository.cs
│   ├── UserRepository.cs
│   └── TaskRepository.cs
├── Services
│   ├── IUserService.cs
│   ├── ITaskService.cs
│   ├── UserService.cs
│   └── TaskService.cs
├── Data
│   ├── ApplicationDbContext.cs
├── Migrations
├── Properties
│   └── launchSettings.json
├── appsettings.json
├── Program.cs
├── Startup.cs
└── UserTasksAPI.csproj
```

## Setup Instructions
1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```
   cd UserTasksAPI
   ```
3. Restore the dependencies:
   ```
   dotnet restore
   ```
4. Update the `appsettings.json` file with your database connection string and any other necessary configurations.
5. Run the migrations to set up the database:
   ```
   dotnet ef database update
   ```
6. Start the application:
   ```
   dotnet run
   ```

## API Usage
- **Base URL**: `http://localhost:<port>/api`
- **Authentication**: Use either API key or JWT tokens for accessing the endpoints.

## Testing
Unit tests are included in the project. Use the following command to run the tests:
```
dotnet test
```

## Documentation
API documentation is available via Swagger. Once the application is running, navigate to `http://localhost:<port>/swagger` to view the API documentation.

## License
This project is licensed under the MIT License.