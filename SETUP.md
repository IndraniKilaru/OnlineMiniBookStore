# Quick Setup Guide

## Prerequisites

1. **Start Infrastructure Services**
   ```bash
   # Start RabbitMQ, Redis, and SQL Server with Docker
   docker-compose up -d sqlserver redis rabbitmq
   ```
   
   **OR use SQL Server LocalDB for development:**
   ```bash
   sqllocaldb start mssqllocaldb
   ```

## Backend Services

**Run each service in separate terminals:**

   **Terminal 1 - Auth Service:**
   ```bash
   cd backend\BookStore.AuthService
   dotnet run --urls "http://localhost:5001"
   ```

   **Terminal 2 - Books Service:**
   ```bash
   cd backend\BookStore.BooksService  
   dotnet run --urls "http://localhost:5002"
   ```

   **Terminal 3 - Cart Service:**
   ```bash
   cd backend\BookStore.CartService
   dotnet run --urls "http://localhost:5003"
   ```

   **Terminal 4 - Orders Service:**
   ```bash
   cd backend\BookStore.OrdersService
   dotnet run --urls "http://localhost:5004"
   ```

   **Terminal 5 - Notification Service:**
   ```bash
   cd backend\BookStore.NotificationService
   dotnet run --urls "http://localhost:5005"
   ```

   **Terminal 6 - API Gateway:**
   ```bash
   cd backend\BookStore.ApiGateway
   dotnet run --urls "http://localhost:5000"
   ```

## Frontend

**Terminal 7 - Angular App:**
```bash
cd frontend
npm install
npm start
# Will run on http://localhost:4201
```

## Test the Application

1. **Frontend:** http://localhost:4201
2. **API Gateway:** http://localhost:5000
3. **RabbitMQ Management:** http://localhost:15672 (bookstore/bookstore123)

## Default Admin Login
- **Email:** admin@bookstore.com
- **Password:** Admin123!

## API Endpoints

### Authentication
- POST `/api/auth/login` - User login
- POST `/api/auth/register` - User registration

### Books
- GET `/api/books` - Get all books (with pagination, search, filtering)
- GET `/api/books/{id}` - Get book by ID
- POST `/api/books` - Create book (Admin only)
- PUT `/api/books/{id}` - Update book (Admin only)
- DELETE `/api/books/{id}` - Delete book (Admin only)

### Cart
- GET `/api/cart` - Get user's cart
- POST `/api/cart/add` - Add item to cart
- PUT `/api/cart/{id}` - Update cart item
- DELETE `/api/cart/{id}` - Remove item from cart

### Orders
- GET `/api/orders` - Get user's orders
- POST `/api/orders` - Create new order
- GET `/api/orders/{id}` - Get order details

## Docker Deployment

```bash
docker-compose up --build
```

This will start all services with:
- SQL Server
- Redis
- RabbitMQ
- All microservices
- Angular frontend
