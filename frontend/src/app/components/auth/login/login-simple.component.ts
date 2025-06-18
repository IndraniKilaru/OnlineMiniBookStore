import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-header">
          <h2>🔐 Login</h2>
          <p>Welcome back to Mini Bookstore!</p>
        </div>
        
        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label for="email">Email</label>
            <input type="email" 
                   id="email"
                   [(ngModel)]="loginData.email" 
                   name="email"
                   required 
                   placeholder="Enter your email"
                   class="form-input">
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input type="password" 
                   id="password"
                   [(ngModel)]="loginData.password" 
                   name="password"
                   required 
                   placeholder="Enter your password"
                   class="form-input">
          </div>

          <div class="backend-notice" *ngIf="!isBackendConnected">
            <p>⚠️ Backend not connected. Demo credentials:</p>
            <p><strong>Admin:</strong> admin@bookstore.com / Admin@123</p>
            <p><strong>User:</strong> user@bookstore.com / User@123</p>
          </div>

          <button type="submit" 
                  [disabled]="isLoading || !loginData.email || !loginData.password"
                  class="btn btn-primary full-width">
            <span *ngIf="!isLoading">Login</span>
            <span *ngIf="isLoading">Logging in...</span>
          </button>
        </form>
        
        <div class="auth-footer">
          <p>Don't have an account? 
            <a routerLink="/register" class="link">Register here</a>
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 80vh;
      padding: 2rem;
      background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
    }
    
    .auth-card {
      background: white;
      width: 100%;
      max-width: 400px;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    }
    
    .auth-header {
      text-align: center;
      margin-bottom: 2rem;
    }
    
    .auth-header h2 {
      margin: 0 0 0.5rem 0;
      color: #333;
      font-size: 2rem;
    }
    
    .auth-header p {
      margin: 0;
      color: #666;
    }
    
    .form-group {
      margin-bottom: 1.5rem;
    }
    
    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      color: #333;
      font-weight: 500;
    }
    
    .form-input {
      width: 100%;
      padding: 0.8rem;
      border: 2px solid #ddd;
      border-radius: 8px;
      font-size: 1rem;
      transition: border-color 0.3s ease;
      box-sizing: border-box;
    }
    
    .form-input:focus {
      outline: none;
      border-color: #667eea;
    }
    
    .backend-notice {
      background: #fff3cd;
      border: 1px solid #ffeeba;
      border-radius: 8px;
      padding: 1rem;
      margin-bottom: 1.5rem;
      font-size: 0.9rem;
    }
    
    .backend-notice p {
      margin: 0.3rem 0;
      color: #856404;
    }
    
    .btn {
      padding: 0.8rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
    }
    
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    
    .btn-primary {
      background: #667eea;
      color: white;
    }
    
    .btn-primary:hover:not(:disabled) {
      background: #5a6fd8;
      transform: translateY(-2px);
    }
    
    .full-width {
      width: 100%;
    }
    
    .auth-footer {
      text-align: center;
      margin-top: 1.5rem;
      padding-top: 1.5rem;
      border-top: 1px solid #eee;
    }
    
    .auth-footer p {
      margin: 0;
      color: #666;
    }
    
    .link {
      color: #667eea;
      text-decoration: none;
      font-weight: 600;
    }
    
    .link:hover {
      text-decoration: underline;
    }
    
    @media (max-width: 768px) {
      .auth-container {
        padding: 1rem;
      }
      
      .auth-card {
        padding: 1.5rem;
      }
    }
  `]
})
export class LoginComponent {
  loginData = {
    email: '',
    password: ''
  };
  
  isLoading = false;
  isBackendConnected = false; // Set to true when backend is connected

  constructor(private router: Router) {}

  onSubmit(): void {
    if (!this.loginData.email || !this.loginData.password) {
      return;
    }

    this.isLoading = true;

    // Simulate API call
    setTimeout(() => {
      this.isLoading = false;
      
      if (this.isBackendConnected) {
        // Real login logic will go here
        alert('Login functionality will work when backend is connected!');
      } else {
        // Demo mode
        if (this.loginData.email === 'admin@bookstore.com' && this.loginData.password === 'Admin@123') {
          alert('Demo Admin Login Successful!\\n\\nRedirecting to home...');
          this.router.navigate(['/']);
        } else if (this.loginData.email === 'user@bookstore.com' && this.loginData.password === 'User@123') {
          alert('Demo User Login Successful!\\n\\nRedirecting to home...');
          this.router.navigate(['/']);
        } else {
          alert('Demo mode: Use the provided credentials or start the backend for real authentication.');
        }
      }
    }, 1000);
  }
}
