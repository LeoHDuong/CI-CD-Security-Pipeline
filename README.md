# MyApp - Full Stack Web Application

A modern full-stack web application built with React, TypeScript, .NET 8, and PostgreSQL.

## 🏗️ Architecture

- **Frontend**: React 19 + TypeScript + Vite
- **Backend**: ASP.NET Core 8 Web API
- **Database**: PostgreSQL 15
- **Containerization**: Docker & Docker Compose

## 🚀 Quick Start

### Prerequisites

- Docker & Docker Compose
- Git

### Setup

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd swe-project
   ```

2. **Environment Setup**
   ```bash
   cp .env.example .env
   # Edit .env with your preferred database credentials
   ```

3. **Start the Application**
   ```bash
   docker-compose up --build
   ```

4. **Access the Application**
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:5105
   - Database: localhost:5432

## 🛠️ Development

### Frontend
```bash
cd frontend
npm install
npm run dev
```

### Backend
```bash
cd backend/MyApp.Api
dotnet restore
dotnet run
```

### Database
The PostgreSQL database runs in a Docker container and persists data using named volumes.

## 📁 Project Structure

```
├── backend/                 # ASP.NET Core API
│   └── MyApp.Api/          # Main API project
├── frontend/               # React application
├── docker-compose.yml      # Container orchestration
├── .env.example           # Environment variables template
└── .gitignore            # Git ignore rules
```

## 🔧 Configuration

### Environment Variables

Copy `.env.example` to `.env` and configure:

- `POSTGRES_DB`: Database name
- `POSTGRES_USER`: Database username
- `POSTGRES_PASSWORD`: Database password
- `DB_HOST`: Database host (default: db)
- `DB_PORT`: Database port (default: 5432)

## 🐳 Docker Services

- **backend**: ASP.NET Core API (port 5105)
- **frontend**: React SPA served by Nginx (port 5173)
- **db**: PostgreSQL database (port 5432)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test with Docker Compose
5. Submit a pull request

## 📝 License

This project is open source and available under the [MIT License](LICENSE).