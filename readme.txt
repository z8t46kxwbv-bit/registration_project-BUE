# User Registration System

A beautiful, full-stack web application for managing user registrations. Built with love using React.js and ASP.NET Core.

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![React](https://img.shields.io/badge/React-18.2.0-61dafb.svg)
![.NET](https://img.shields.io/badge/.NET-6.0-512bd4.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

This isn't just another CRUD app - it's a thoughtfully designed system that treats users like humans, not database entries!



**Frontend Magic**
- **Responsive Design** - Looks great on phones, tablets, and desktops
- **Real-time Validation** - Friendly error messages as you type
- **Smart Search** - Find anyone by name or email instantly
- **Smooth Pagination** - Navigate through users effortlessly
- **Live Editing** - Update user info on the fly
- **Beautiful UI** - Modern gradient design with Tailwind CSS
- **Helpful Messages** - Success notifications and encouraging error messages

**Backend Power** 
- **RESTful API** - Clean, industry-standard endpoints
- **Clean Architecture** - Separated layers for maintainability
- **Bulletproof Validation** - Server-side checks for data integrity
- **Smart Error Handling** - Proper HTTP status codes and meaningful responses
- **Repository Pattern** - Easy to test and maintain
- **Entity Framework** - Smooth database operations

**Database Excellence*
- **SQL Server** - Enterprise-grade reliability
- **Smart Constraints** - Unique emails, proper data types
- **Optimized Indexes** - Fast queries even with millions of users
- **Audit Trail** - CreatedAt and UpdatedAt timestamps

---

## Quick Start

### Prerequisites

Before you begin, make sure you have:

- **Node.js** (v16 or higher) - [Download here](https://nodejs.org/)
- **.NET 6 SDK** or higher - [Download here](https://dotnet.microsoft.com/download)
- **SQL Server** (LocalDB, Express, or Full) - [Get it here](https://www.microsoft.com/sql-server)
- **Your favorite code editor** (VS Code, Visual Studio, etc.)

#### Step 1: Get the Code
```bash
git clone https://github.com/yourusername/registration-system.git
cd registration-system
```

#### Step 2: Set Up the Backend

```bash
# Navigate to the API project
cd RegistrationSystem.API

# Install dependencies
dotnet restore

# Update the database connection string in appsettings.json
# Then create the database
dotnet ef database update --project ../RegistrationSystem.Infrastructure

# Fire up the backend! 
dotnet run
```

Your API will be running at `https://localhost:5001`

#### Step 3: Set Up the Frontend

```bash
# Open a new terminal and go to the React app
cd registration-app

# Install all the goodies
npm install

# Start the development server
npm start
```

Your app will open at `http://localhost:3000` - Magic! 

---

##  Project Structure

Here's how everything is organized:

```
RegistrationSystem/
│
├──  registration-app/              # React Frontend
│   ├── src/
│   │   ├── App.js                    # Main application component
│   │   ├── index.js                  # Entry point
│   │   └── index.css                 # Tailwind styles
│   ├── public/
│   └── package.json
│
├──  RegistrationSystem.API/        # Web API Layer
│   ├── Controllers/
│   │   └── UsersController.cs        # API endpoints
│   ├── Models/
│   │   ├── DTOs/                     # Data transfer objects
│   │   └── Responses/                # API response models
│   ├── Program.cs                    # App configuration
│   └── appsettings.json              # Settings
│
├──  RegistrationSystem.Core/       # Business Logic
│   ├── Entities/
│   │   └── User.cs                   # User model
│   ├── Interfaces/                   # Contracts
│   └── Services/
│       └── UserService.cs            # Business rules
│
├── ️ RegistrationSystem.Infrastructure/  # Data Access
│   ├── Data/
│   │   └── ApplicationDbContext.cs   # EF Core context
│   ├── Repositories/
│   │   └── UserRepository.cs         # Database operations
│   └── Migrations/                   # Database migrations
│
└──  RegistrationSystem.Tests/      # Unit Tests
    └── Services/
        └── UserServiceTests.cs
```

---

##API Documentation

### Available Endpoints

| Method | Endpoint | What It Does | Example |
|--------|----------|--------------|---------|
| `GET` | `/api/users` | Get all users (with pagination) | `GET /api/users?page=1&pageSize=10&search=john` |
| `GET` | `/api/users/{id}` | Get a specific user | `GET /api/users/5` |
| `POST` | `/api/users` | Create a new user | See below |
| `PUT` | `/api/users/{id}` | Update a user | See below |
| `DELETE` | `/api/users/{id}` | Delete a user | `DELETE /api/users/5` |

### Example Requests

**Creating a New User:**
```json
POST /api/users
Content-Type: application/json

{
  "name": "Sarah",
  "email": "sarah.j@email.com",
  "phone": "+1 (555) 234-5678",
  "age": 28
}
```

**Response:**
```json
{
  "success": true,
  "message": "User created successfully",
  "data": {
    "id": 1,
    "name": "Sarah ",
    "email": "sarah.j@email.com",
    "phone": "+1 (555) 234-5678",
    "age": 28,
    "createdAt": "2024-01-20T10:30:00Z"
  }
}
```

**Searching Users:**
```bash
GET /api/users?page=1&pageSize=10&search=sarah
```

---

## Frontend Features in Detail

### Registration Form

The form includes smart validation for:

- **Name** (2-100 characters)
  - Friendly message: "Hey, we need a name here!"
  
- **Email** (valid format, unique)
  - Checks: "That doesn't look like a valid email address"
  
- **Phone** (minimum 11 digits)
  - Example: "+1 (555) 123-4567"
  
- **Age** (1-150)
  - Validates: "Age should be between 1 and 150"

### User List View

- **Desktop**: Beautiful table layout
- **Mobile**: Card-based design
- **Search**: Filter by name or email in real-time
- **Pagination**: 5 users per page by default
- **Quick Edit**: Click to edit any user instantly

---

## Database Schema

### Users Table

| Column | Type | Description | Constraints |
|--------|------|-------------|-------------|
| `Id` | INT | Unique identifier | Primary Key, Auto-increment |
| `Name` | NVARCHAR(100) | Full name | NOT NULL, 2-100 chars |
| `Email` | NVARCHAR(100) | Email address | NOT NULL, UNIQUE, Valid format |
| `Phone` | NVARCHAR(20) | Phone number | NOT NULL, Min 11 digits |
| `Age` | INT | Age in years | NOT NULL, 1-150 |
| `CreatedAt` | DATETIME2 | Registration date | NOT NULL, Auto-set |
| `UpdatedAt` | DATETIME2 | Last update | NULL |

**Indexes:**
- Unique index on `Email`
- Clustered index on `Id`

---

## Running Tests

We've included unit tests to ensure everything works perfectly!

```bash
# Navigate to the test project
cd RegistrationSystem.Tests

# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Check code coverage
dotnet test /p:CollectCoverage=true
```

### What's Tested?

-  Creating new users
-  Updating existing users
-  Email uniqueness validation
-  Getting users by ID
-  Error handling

---

## Configuration

### Backend Configuration

Edit `RegistrationSystem.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RegistrationSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**For SQL Server Express:**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=RegistrationSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**For Azure SQL:**
```json
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=RegistrationSystemDb;User ID=yourusername;Password=yourpassword;Encrypt=True;"
```

### Frontend Configuration

In the React app, toggle between mock and real data:

```javascript
// In App.js
const USE_MOCK_DATA = true;  // Set to false to use real backend
const API_URL = 'https://localhost:5001/api';
```

---
### For End Users

1. **Adding a New User**
   - Click "Add New User"
   - Fill in all required fields
   - Click "Register User"
   - See success message!

2. **Searching for Someone**
   - Go to "View Everyone"
   - Type in the search box
   - Results filter instantly

3. **Editing a User**
   - Find the user in the list
   - Click "Edit"
   - Make your changes
   - Click "Update User"

### For Developers

**Adding a New Field:**

1. Update `User.cs` entity
2. Create a migration: `dotnet ef migrations add AddNewField`
3. Update database: `dotnet ef database update`
4. Update DTOs in the API layer
5. Update frontend form fields

**Changing Validation Rules:**

- Frontend: Modify `checkFormValidity()` function
- Backend: Update data annotations in DTOs

---

## Troubleshooting

### Common Issues & Solutions

** "Cannot connect to database"**
-  Make sure SQL Server is running
-  Check your connection string
-  Verify the database exists

** "CORS policy error"**
-  Check the CORS policy in `Program.cs`
-  Verify frontend URL matches allowed origins
-  Ensure both servers are running

** "API not responding"**
-  Confirm API is running on port 5001
-  Check firewall settings
-  Look at API console for errors

**"React app won't start"**
-  Delete `node_modules` and run `npm install` again
-  Clear npm cache: `npm cache clean --force`
-  Check Node.js version: `node --version`

** "Migration failed"**
-  Delete the Migrations folder
-  Drop the database
-  Create new migration: `dotnet ef migrations add Initial`
-  Update database: `dotnet ef database update`

---


### Clean Architecture Layers

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│    (React App + API Controllers)    │
├─────────────────────────────────────┤
│         Application Layer           │
│      (Services, DTOs, Interfaces)   │
├─────────────────────────────────────┤
│           Domain Layer              │
│         (Entities, Rules)           │
├─────────────────────────────────────┤
│       Infrastructure Layer          │
│   (Repositories, Database Context)  │
└─────────────────────────────────────┘
```

### Design Patterns Used

- **Repository Pattern** - Abstraction over data access
- **Service Layer Pattern** - Business logic separation
- **DTO Pattern** - Data transfer between layers
- **Dependency Injection** - Loose coupling
- **Factory Pattern** - Object creation

---



### Deploying the Backend

**To Azure App Service:**
```bash
# Publish the app
dotnet publish -c Release

# Deploy to Azure (requires Azure CLI)
az webapp up --name your-app-name --resource-group your-rg
```

**To IIS:**
1. Publish the app to a folder
2. Copy to IIS wwwroot
3. Configure application pool (.NET CLR Version: No Managed Code)
4. Update connection string in production appsettings

### Deploying the Frontend

**To Netlify:**
```bash
# Build the app
npm run build

# Deploy (requires Netlify CLI)
netlify deploy --prod --dir=build
```

**To Vercel:**
```bash
# Install Vercel CLI
npm i -g vercel

# Deploy
vercel --prod
```

---
### Backend Optimization

- Enable response compression
- Add caching for frequently accessed data
- Use pagination (already implemented!)
- Add database indexes on search columns
- Consider adding Redis for session storage

### Frontend Optimization

- Use React.memo for expensive components
- Implement virtual scrolling for large lists
- Add debouncing to search input
- Lazy load components
- Optimize images and assets

---



### How to Contribute

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit your changes** (`git commit -m 'Add some AmazingFeature'`)
4. **Push to the branch** (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

### Coding Standards

- Write clean, readable code
- Add comments for complex logic
- Follow existing naming conventions
- Write tests for new features
- Update documentation


##Acknowledgments

- **React Team** - For the amazing framework
- **Microsoft** - For .NET and Entity Framework
- **Tailwind CSS** - For beautiful, utility-first styling
- **Lucide Icons** - For gorgeous icons
- **You** - For using this project!


## Roadmap

- [ ] **Authentication & Authorization** - JWT-based login system
- [ ] **Role Management** - Admin, User, Viewer roles
- [ ] **Advanced Filtering** - Filter by age range, registration date
- [ ] **Bulk Operations** - Import/export users via CSV
- [ ] **Email Notifications** - Welcome emails for new users
- [ ] **Profile Pictures** - Upload and display user avatars
- [ ] **Activity Logs** - Track all user actions
- [ ] **Dark Mode** - Because everyone loves dark mode
- [ ] **Mobile App** - React Native version
- [ ] **Real-time Updates** - Using SignalR


