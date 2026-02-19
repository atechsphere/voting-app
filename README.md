# .NET Voting Application

A secure online voting application built with .NET 9, ASP.NET Core MVC, Entity Framework Core, and MySQL.

## Features

- **User Registration & Authentication**: Secure password hashing with ASP.NET Identity
- **Voting System**: One vote per user, enforced at database and application level
- **Real-time Results**: Live vote counting and percentage display
- **Responsive Design**: Bootstrap 5 based UI that works on all devices
- **Docker Containerization**: Easy deployment with Docker and Docker Compose
- **CI/CD Pipeline**: Automated Jenkins pipeline for building and deployment

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Docker Network (voting-net)               │
│  ┌────────────────────┐      ┌────────────────────┐        │
│  │   Voting App       │      │      MySQL         │        │
│  │   (.NET 9)         │──────│      (8.0)         │        │
│  │   Port: 8087       │      │      Port: 3306    │        │
│  └────────────────────┘      └────────────────────┘        │
│           │                           │                     │
│           └───────────────────────────┘                     │
│                    voting-net                                │
└─────────────────────────────────────────────────────────────┘
```

## Prerequisites

- Ubuntu 20.04+ (or similar Linux distribution)
- Docker Engine 24.0+
- Docker Compose 2.0+
- .NET 9 SDK (for local development)
- Jenkins (for CI/CD)

## Quick Start

### 1. Setup Local Registry

```bash
./setup-local-registry.sh
```

### 2. Build and Push Images

```bash
./build-and-push.sh
```

### 3. Deploy Application

```bash
./deploy.sh
```

### 4. Access Application

Open your browser and navigate to: **http://localhost:8087**

## Project Structure

```
voting-app/
├── VotingApp/                 # .NET MVC Application
│   ├── Controllers/           # MVC Controllers
│   ├── Models/                # Data Models & ViewModels
│   ├── Views/                 # Razor Views
│   ├── Data/                  # Entity Framework DbContext & Migrations
│   ├── Services/              # Business Logic Services
│   ├── wwwroot/               # Static Files (CSS, JS)
│   ├── Dockerfile             # Container definition
│   └── VotingApp.csproj       # Project file
├── jenkins/                   # Jenkins configuration
│   ├── Dockerfile             # Jenkins with Docker & .NET
│   └── jenkins-compose.yml    # Jenkins deployment
├── mysql-init/                # MySQL initialization scripts
├── docker-compose.yml         # Application deployment
├── Jenkinsfile                # CI/CD Pipeline definition
├── setup-local-registry.sh    # Registry setup script
├── build-and-push.sh          # Build & push script
├── deploy.sh                  # Deployment script
└── cleanup.sh                 # Cleanup script
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Application environment | Production |
| `ASPNETCORE_URLS` | Application URLs | http://+:8087 |
| `ConnectionStrings__DefaultConnection` | MySQL connection string | See docker-compose.yml |

### Database Schema

#### Users Table (AspNetUsers)
- Standard ASP.NET Identity columns
- `FirstName`, `LastName` - User name
- `CreatedAt` - Account creation timestamp
- `VotedAt` - Vote timestamp

#### Votes Table
- `Id` - Primary key
- `UserId` - Foreign key to Users (unique constraint)
- `Candidate` - Selected candidate
- `VotedAt` - Vote timestamp

## Jenkins Pipeline

The Jenkins pipeline automates:

1. **Checkout**: Pull latest code
2. **Setup Registry**: Ensure local Docker registry is running
3. **Build**: Compile .NET application
4. **Docker Build**: Create container images
5. **Security Scan**: Trivy vulnerability scanning
6. **Push**: Push images to local registry
7. **Deploy**: Deploy with Docker Compose
8. **Health Check**: Verify application is running

### Running Jenkins

```bash
cd jenkins
<<<<<<< HEAD
docker compose  -f jenkins-compose.yml up -d
=======
docker-compose -f jenkins-compose.yml up -d
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
```

Access Jenkins at: **http://localhost:8080**

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/` | GET | Home page with voting results |
| `/Account/Register` | GET/POST | User registration |
| `/Account/Login` | GET/POST | User login |
| `/Account/Logout` | POST | User logout |
| `/Dashboard` | GET | User dashboard |
| `/Dashboard/Vote` | POST | Cast vote |

## Development

### Local Development

```bash
cd VotingApp
dotnet restore
dotnet run
```

### Run Tests

```bash
cd VotingApp
dotnet test
```

### Build Docker Image

```bash
cd VotingApp
docker build -t voting-app:dev .
```

## Security Features

- ASP.NET Identity for secure password hashing
- CSRF protection with anti-forgery tokens
- SQL injection prevention via Entity Framework
- Cookie-based authentication with secure settings
- Non-root container user
- Unique constraint on user votes

## Troubleshooting

### View Logs

```bash
# All services
<<<<<<< HEAD
docker compose  logs -f

# Specific service
docker compose  logs -f voting-app
docker compose  logs -f mysql
=======
docker-compose logs -f

# Specific service
docker-compose logs -f voting-app
docker-compose logs -f mysql
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
```

### Common Issues

1. **MySQL Connection Failed**
   - Wait for MySQL to be ready (health check)
   - Check credentials in docker-compose.yml

2. **Application Won't Start**
<<<<<<< HEAD
   - Check logs: `docker compose  logs voting-app`
=======
   - Check logs: `docker-compose logs voting-app`
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
   - Verify database connection

3. **Registry Not Accessible**
   - Restart registry: `./setup-local-registry.sh`

## Cleanup

```bash
./cleanup.sh
```

## License

MIT License

## Authors

Generated by Super Z - AI Assistant
