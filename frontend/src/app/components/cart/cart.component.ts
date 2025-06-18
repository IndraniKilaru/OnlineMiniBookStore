import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { CartService, CartItem } from '../../services/cart.service';
import { OrdersService, CreateOrderRequest } from '../../services/orders.service';

@Component({
  selector: 'app-cart',
  template: `
    <div class="cart-container">
      <h1>Shopping Cart</h1>

      <div *ngIf="isLoading" class="loading-container">
        <mat-spinner></mat-spinner>
      </div>

      <div *ngIf="!isLoading && cartItems.length === 0" class="empty-cart">
        <mat-icon>shopping_cart</mat-icon>
        <h3>Your cart is empty</h3>
        <p>Add some books to get started!</p>
        <button mat-raised-button color="primary" routerLink="/books">
          Browse Books
        </button>
      </div>

      <div *ngIf="!isLoading && cartItems.length > 0">
        <div class="cart-items">
          <div *ngFor="let item of cartItems" class="cart-item">
            <img [src]="getImageUrl(item.book.imageUrl)" [alt]="item.book.title" class="item-image" (error)="onImageError($event)">
            
            <div class="item-details">
              <h3>{{item.book.title}}</h3>
              <p>by {{item.book.author}}</p>
              <span class="item-price">\${{item.book.price}}</span>
            </div>

            <div class="quantity-controls">
              <button mat-icon-button (click)="updateQuantity(item, item.quantity - 1)" 
                      [disabled]="item.quantity <= 1">
                <mat-icon>remove</mat-icon>
              </button>
              <span class="quantity">{{item.quantity}}</span>
              <button mat-icon-button (click)="updateQuantity(item, item.quantity + 1)">
                <mat-icon>add</mat-icon>
              </button>
            </div>

            <div class="item-total">
              <span>\${{(item.book.price * item.quantity).toFixed(2)}}</span>
            </div>

            <button mat-icon-button color="warn" (click)="removeItem(item)">
              <mat-icon>delete</mat-icon>
            </button>
          </div>
        </div>

        <div class="cart-summary">
          <div class="total-section">
            <h3>Total: \${{getTotal().toFixed(2)}}</h3>
          </div>
            <div class="actions">
            <button mat-button (click)="clearCart()">Clear Cart</button>
            <button mat-raised-button color="primary" 
                    (click)="checkout()" 
                    [disabled]="isCheckingOut || cartItems.length === 0">
              <span *ngIf="!isCheckingOut">Proceed to Checkout</span>
              <span *ngIf="isCheckingOut">Processing...</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .cart-container {
      padding: 20px;
      max-width: 800px;
      margin: 0 auto;
    }

    .empty-cart {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .empty-cart mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      margin-bottom: 16px;
    }

    .cart-items {
      margin-bottom: 24px;
    }

    .cart-item {
      display: flex;
      align-items: center;
      padding: 16px;
      border: 1px solid #eee;
      border-radius: 8px;
      margin-bottom: 16px;
      gap: 16px;
    }

    .item-image {
      width: 80px;
      height: 100px;
      object-fit: cover;
      border-radius: 4px;
    }

    .item-details {
      flex: 1;
    }

    .item-details h3 {
      margin: 0 0 4px 0;
      font-size: 1.1em;
    }

    .item-details p {
      margin: 0 0 8px 0;
      color: #666;
    }

    .item-price {
      font-weight: bold;
      color: #1976d2;
    }

    .quantity-controls {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .quantity {
      font-weight: bold;
      min-width: 30px;
      text-align: center;
    }

    .item-total {
      font-weight: bold;
      font-size: 1.1em;
      color: #1976d2;
      min-width: 80px;
      text-align: right;
    }

    .cart-summary {
      border-top: 2px solid #eee;
      padding-top: 24px;
    }

    .total-section {
      text-align: right;
      margin-bottom: 16px;
    }

    .total-section h3 {
      font-size: 1.5em;
      color: #1976d2;
    }

    .actions {
      display: flex;
      justify-content: space-between;
      gap: 16px;
    }

    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 200px;
    }
  `]
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  isLoading = false;
  isCheckingOut = false;

  constructor(
    private cartService: CartService,
    private ordersService: OrdersService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.loadCart();
  }
  loadCart() {
    this.isLoading = true;
    this.cartService.getCart().subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          this.cartItems = response.data.items || [];
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open('Failed to load cart', 'Close', { duration: 3000 });
        console.error('Error loading cart:', error);
      }
    });
  }

  updateQuantity(item: CartItem, newQuantity: number) {
    if (newQuantity < 1) return;

    this.cartService.updateCartItem(item.id, newQuantity).subscribe({
      next: () => {
        item.quantity = newQuantity;
        this.snackBar.open('Cart updated', 'Close', { duration: 2000 });
      },
      error: (error) => {
        this.snackBar.open('Failed to update cart', 'Close', { duration: 3000 });
        console.error('Error updating cart:', error);
      }
    });
  }

  removeItem(item: CartItem) {
    this.cartService.removeFromCart(item.id).subscribe({
      next: () => {
        this.cartItems = this.cartItems.filter(i => i.id !== item.id);
        this.snackBar.open('Item removed from cart', 'Close', { duration: 2000 });
      },
      error: (error) => {
        this.snackBar.open('Failed to remove item', 'Close', { duration: 3000 });
        console.error('Error removing item:', error);
      }
    });
  }

  clearCart() {
    this.cartService.clearCart().subscribe({
      next: () => {
        this.cartItems = [];
        this.snackBar.open('Cart cleared', 'Close', { duration: 2000 });
      },
      error: (error) => {
        this.snackBar.open('Failed to clear cart', 'Close', { duration: 3000 });
        console.error('Error clearing cart:', error);
      }
    });
  }

  getTotal(): number {
    return this.cartItems.reduce((total, item) => total + (item.book.price * item.quantity), 0);
  }
  checkout() {
    if (this.cartItems.length === 0) {
      this.snackBar.open('Your cart is empty', 'Close', { duration: 3000 });
      return;
    }

    this.isCheckingOut = true;

    // Create order request from cart items
    const orderRequest: CreateOrderRequest = {
      items: this.cartItems.map(item => ({
        bookId: item.bookId,
        quantity: item.quantity
      }))
    };

    this.ordersService.createOrder(orderRequest).subscribe({
      next: (response) => {
        this.isCheckingOut = false;
        if (response.success) {
          this.snackBar.open('Order placed successfully!', 'Close', { duration: 3000 });
          
          // Clear the cart after successful order
          this.cartService.clearCart().subscribe({
            next: () => {
              this.cartItems = [];
              // Navigate to orders page to show the new order
              this.router.navigate(['/orders']);
            },
            error: (error) => {
              console.error('Error clearing cart after order:', error);
              // Still navigate to orders even if cart clear fails
              this.router.navigate(['/orders']);
            }
          });
        } else {
          this.snackBar.open('Failed to place order', 'Close', { duration: 3000 });
        }
      },      error: (error) => {
        this.isCheckingOut = false;
        this.snackBar.open('Failed to place order', 'Close', { duration: 3000 });
        console.error('Error placing order:', error);
      }
    });
  }

  getImageUrl(imageUrl: string): string {
    // If no image URL or if it's a known problematic URL, use placeholder
    if (!imageUrl || imageUrl.includes('openlibrary.org/b/id/8225261')) {
      return '/assets/book-placeholder-2.svg';
    }
    return imageUrl;
  }

  onImageError(event: any) {
    event.target.src = '/assets/book-placeholder-2.svg';
  }
}
