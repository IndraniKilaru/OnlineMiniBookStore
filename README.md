# Mini Online Bookstore

A complete e-commerce solution built with ASP.NET Core microservices and Angular frontend.

## Architecture

### Backend (ASP.NET Core Microservices)
- **API Gateway**: Routes requests to appropriate services
- **Authentication Service**: Handles user registration, login, and JWT token management
- **Books Service**: Manages book catalog, categories, and inventory
- **Cart Service**: Handles shopping cart operations with Redis caching
- **Orders Service**: Processes orders and integrates with RabbitMQ for messaging
- **Notification Service**: Processes events and sends email notifications

### Frontend (Angular 15+)
- Modern responsive UI with Angular Material
- Role-based access control (Admin/Customer)
- Shopping cart functionality
- Order management

### Infrastructure
- **Docker**: Containerization for all services
- **SQL Server**: Primary database for each service
- **Redis**: Caching for cart operations
- **RabbitMQ**: Event-driven messaging for notifications

## Features

### For Customers
- Browse books with pagination and search
- Filter books by category
- Add books to cart
- Place orders
- View order history

### For Admins
- Add new books to catalog
- Update book information
- Manage inventory
- View all orders

## Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 16+
- Docker Desktop
- SQL Server LocalDB (for local development)

### Local Development

1. **Start Infrastructure (Required first)**
   ```bash
   # Start RabbitMQ, Redis, and SQL Server
   docker-compose up -d sqlserver redis rabbitmq
   ```

2. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MiniOnlineBookStore
   ```

3. **Backend Setup**
   ```bash
   cd backend
   dotnet restore
   dotnet build
   ```

4. **Start the services**
   ```bash
   # Start services individually in separate terminals:
   
   # Auth Service (Port 5001)
   cd BookStore.AuthService
   dotnet run

   # Books Service (Port 5002)  
   cd ../BookStore.BooksService
   dotnet run

   # Cart Service (Port 5003)
   cd ../BookStore.CartService
   dotnet run

   # Orders Service (Port 5004)
   cd ../BookStore.OrdersService
   dotnet run

   # Notification Service (Port 5005)
   cd ../BookStore.NotificationService
   dotnet run

   # API Gateway (Port 5000)
   cd ../BookStore.ApiGateway
   dotnet run
   ```

5. **Frontend Setup**
   ```bash
   cd frontend
   npm install
   ng serve --port 4201
   ```

### Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

2. **Services will be available at:**
   - Frontend: http://localhost:4201
   - API Gateway: http://localhost:5000
   - Auth Service: http://localhost:5001
   - Books Service: http://localhost:5002
   - Cart Service: http://localhost:5003
   - Orders Service: http://localhost:5004
   - Notification Service: http://localhost:5005
   - RabbitMQ Management: http://localhost:15672 (bookstore/bookstore123)

## Default Admin Account

- **Email**: admin@bookstore.com
- **Password**: Admin123!

## API Documentation

Each service includes OpenAPI/Swagger documentation available at:
- Auth Service: http://localhost:5001/swagger
- Books Service: http://localhost:5002/swagger
- Cart Service: http://localhost:5003/swagger
- Orders Service: http://localhost:5004/swagger

## RabbitMQ Management

- **URL**: http://localhost:15672
- **Username**: bookstore
- **Password**: bookstore123
- **Queues**: Monitor `bookstore.users` and `bookstore.orders` for message activity

## Database Schema

### Users (Auth Service)
- User management and authentication

### Books (Books Service)
- Book catalog with categories and inventory

### Cart Items (Cart Service)
- Shopping cart persistence with Redis caching

### Orders (Orders Service)
- Order processing and tracking

## Technologies Used

### Backend
- ASP.NET Core 8
- Entity Framework Core
- JWT Authentication
- Redis for caching
- RabbitMQ for messaging
- SQL Server

### Frontend
- Angular 15+
- Angular Material
- TypeScript
- RxJS

### DevOps
- Docker & Docker Compose
- Multi-stage Docker builds

## Development Notes

- Each microservice has its own database
- JWT tokens are used for authentication across services
- CORS is configured for local development
- Redis is used for cart caching
- RabbitMQ handles order processing events

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License.
