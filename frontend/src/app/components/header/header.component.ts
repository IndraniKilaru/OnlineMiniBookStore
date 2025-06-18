import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-header',
  template: `
    <mat-toolbar color="primary">
      <mat-toolbar-row>
        <button mat-button routerLink="/books">
          <mat-icon>book</mat-icon>
          <span>Mini Bookstore</span>
        </button>
        
        <span class="spacer"></span>
        
        <div class="nav-links">
          <button mat-button routerLink="/books">Books</button>
          
          <ng-container *ngIf="isAuthenticated">
            <button mat-button routerLink="/cart" [matBadge]="cartItemCount" 
                    matBadgeColor="warn" [matBadgeHidden]="cartItemCount === 0">
              <mat-icon>shopping_cart</mat-icon>
              Cart
            </button>
            
            <button mat-button routerLink="/orders">
              <mat-icon>receipt</mat-icon>
              Orders
            </button>
            
            <button mat-button *ngIf="isAdmin" routerLink="/admin">
              <mat-icon>admin_panel_settings</mat-icon>
              Admin
            </button>
              <button mat-button [matMenuTriggerFor]="userMenu">
              <mat-icon>account_circle</mat-icon>
              {{userDisplayName}}
              <mat-icon class="role-badge" *ngIf="isAdmin">admin_panel_settings</mat-icon>
            </button>
            
            <mat-menu #userMenu="matMenu">
              <div class="user-info" mat-menu-item disabled>
                <div class="user-details">
                  <strong>{{userDisplayName}}</strong>
                  <small class="role-text">{{userRole}}</small>
                </div>
              </div>
              <mat-divider></mat-divider>
              <button mat-menu-item (click)="logout()">
                <mat-icon>logout</mat-icon>
                Logout
              </button>
            </mat-menu>
          </ng-container>
          
          <ng-container *ngIf="!isAuthenticated">
            <button mat-button routerLink="/login">Login</button>
            <button mat-button routerLink="/register">Register</button>
          </ng-container>
        </div>
      </mat-toolbar-row>
    </mat-toolbar>
  `,
  styles: [`
    .spacer {
      flex: 1 1 auto;
    }
    
    .nav-links {
      display: flex;
      align-items: center;
      gap: 8px;
    }
      .nav-links button {
      margin: 0 4px;
    }
    
    .role-badge {
      font-size: 16px;
      margin-left: 4px;
      color: #ff9800;
    }
    
    .user-info {
      padding: 8px 16px !important;
    }
    
    .user-details {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
    }
    
    .role-text {
      color: #666;
      font-size: 12px;
      text-transform: capitalize;
    }
  `]
})
export class HeaderComponent implements OnInit {
  isAuthenticated = false;
  isAdmin = false;
  userDisplayName = '';
  userRole = '';
  cartItemCount = 0;

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit() {    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
      this.isAdmin = user?.role === 'Admin';
      this.userDisplayName = user ? `${user.firstName} ${user.lastName}` : '';
      this.userRole = user?.role || '';
    });

    this.cartService.cartItemCount$.subscribe(count => {
      this.cartItemCount = count;
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/books']);
  }
}
