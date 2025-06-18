import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrdersService, Order } from '../../services/orders.service';

@Component({
  selector: 'app-orders',
  template: `
    <div class="orders-container">
      <h1>My Orders</h1>

      <div *ngIf="isLoading" class="loading-container">
        <mat-spinner></mat-spinner>
      </div>

      <div *ngIf="!isLoading && orders.length === 0" class="no-orders">
        <mat-icon>receipt</mat-icon>
        <h3>No orders yet</h3>
        <p>You haven't placed any orders yet. Start shopping to see your orders here!</p>
        <button mat-raised-button color="primary" routerLink="/books">
          Browse Books
        </button>
      </div>

      <div *ngIf="!isLoading && orders.length > 0" class="orders-list">
        <div *ngFor="let order of orders" class="order-card">
          <div class="order-header">
            <div class="order-info">
              <h3>Order #{{order.id}}</h3>
              <p class="order-date">{{formatDate(order.createdAt)}}</p>
            </div>
            <div class="order-status">
              <span class="status-badge" [ngClass]="getStatusClass(order.status)">
                {{order.status}}
              </span>
            </div>
          </div>

          <div class="order-items">            <div *ngFor="let item of order.orderItems" class="order-item">
              <img [src]="getImageUrl(item.book?.imageUrl)" 
                   [alt]="item.book?.title" 
                   class="item-image"
                   (error)="onImageError($event, item.book?.title)">
              <div class="item-details">
                <h4>{{item.book?.title || 'Book ' + item.bookId}}</h4>
                <p>by {{item.book?.author || 'Unknown Author'}}</p>
                <p class="item-quantity">Quantity: {{item.quantity}}</p>
              </div>
              <div class="item-price">
                <span>\${{item.totalPrice.toFixed(2)}}</span>
              </div>
            </div>
          </div>

          <div class="order-total">
            <strong>Total: \${{order.totalAmount.toFixed(2)}}</strong>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .orders-container {
      padding: 20px;
      max-width: 1000px;
      margin: 0 auto;
    }

    .loading-container {
      display: flex;
      justify-content: center;
      padding: 40px;
    }

    .no-orders {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .no-orders mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      margin-bottom: 16px;
    }

    .orders-list {
      gap: 20px;
    }

    .order-card {
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      margin-bottom: 20px;
      overflow: hidden;
    }

    .order-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 20px;
      background: #f5f5f5;
      border-bottom: 1px solid #e0e0e0;
    }

    .order-info h3 {
      margin: 0 0 4px 0;
      font-size: 1.2em;
    }

    .order-date {
      margin: 0;
      color: #666;
      font-size: 0.9em;
    }

    .status-badge {
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 0.85em;
      font-weight: bold;
      text-transform: uppercase;
    }

    .status-pending {
      background: #fff3cd;
      color: #856404;
    }

    .status-processing {
      background: #d1ecf1;
      color: #0c5460;
    }

    .status-shipped {
      background: #d4edda;
      color: #155724;
    }

    .status-delivered {
      background: #d4edda;
      color: #155724;
    }

    .status-cancelled {
      background: #f8d7da;
      color: #721c24;
    }

    .order-items {
      padding: 20px;
    }

    .order-item {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 12px 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .order-item:last-child {
      border-bottom: none;
    }

    .item-image {
      width: 60px;
      height: 75px;
      object-fit: cover;
      border-radius: 4px;
    }

    .item-details {
      flex: 1;
    }

    .item-details h4 {
      margin: 0 0 4px 0;
      font-size: 1.1em;
    }

    .item-details p {
      margin: 0 0 2px 0;
      color: #666;
      font-size: 0.9em;
    }

    .item-quantity {
      font-weight: bold;
    }

    .item-price {
      font-size: 1.1em;
      font-weight: bold;
      color: #2e7d32;
    }

    .order-total {
      padding: 16px 20px;
      background: #f5f5f5;
      border-top: 1px solid #e0e0e0;
      text-align: right;
      font-size: 1.2em;
    }
  `]
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  isLoading = false;

  constructor(
    private ordersService: OrdersService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.isLoading = true;
    this.ordersService.getUserOrders().subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          this.orders = response.data.sort((a, b) => 
            new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
          );
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open('Failed to load orders', 'Close', { duration: 3000 });
        console.error('Error loading orders:', error);
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });  }

  getStatusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  getImageUrl(imageUrl?: string): string {
    // If no image URL or if it's a known problematic URL, use placeholder
    if (!imageUrl || imageUrl.includes('openlibrary.org/b/id/8225261')) {
      return '/assets/book-placeholder-2.svg';
    }
    return imageUrl;
  }

  onImageError(event: any, bookTitle?: string) {
    console.log(`Image load failed for book: ${bookTitle || 'Unknown'}`);
    // Replace broken image with placeholder
    event.target.src = '/assets/book-placeholder-2.svg';
  }
}
