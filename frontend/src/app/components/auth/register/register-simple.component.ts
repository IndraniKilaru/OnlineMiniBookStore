import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-header">
          <h2>📝 Register</h2>
          <p>Join Mini Bookstore today!</p>
        </div>
        
        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label for="firstName">First Name</label>
            <input type="text" 
                   id="firstName"
                   [(ngModel)]="registerData.firstName" 
                   name="firstName"
                   required 
                   placeholder="Enter your first name"
                   class="form-input">
          </div>

          <div class="form-group">
            <label for="lastName">Last Name</label>
            <input type="text" 
                   id="lastName"
                   [(ngModel)]="registerData.lastName" 
                   name="lastName"
                   required 
                   placeholder="Enter your last name"
                   class="form-input">
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input type="email" 
                   id="email"
                   [(ngModel)]="registerData.email" 
                   name="email"
                   required 
                   placeholder="Enter your email"
                   class="form-input">
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input type="password" 
                   id="password"
                   [(ngModel)]="registerData.password" 
                   name="password"
                   required 
                   placeholder="Enter your password"
                   class="form-input">
          </div>

          <div class="form-group">
            <label for="confirmPassword">Confirm Password</label>
            <input type="password" 
                   id="confirmPassword"
                   [(ngModel)]="registerData.confirmPassword" 
                   name="confirmPassword"
                   required 
                   placeholder="Confirm your password"
                   class="form-input">
          </div>

          <div class="backend-notice" *ngIf="!isBackendConnected">
            <p>⚠️ Backend not connected</p>
            <p>Registration will work when backend services are running.</p>
            <p>For now, this is a demo form.</p>
          </div>

          <button type="submit" 
                  [disabled]="isLoading || !isFormValid()"
                  class="btn btn-primary full-width">
            <span *ngIf="!isLoading">Register</span>
            <span *ngIf="isLoading">Creating account...</span>
          </button>
        </form>
        
        <div class="auth-footer">
          <p>Already have an account? 
            <a routerLink="/login" class="link">Login here</a>
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
    
    .form-input.error {
      border-color: #f44336;
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
export class RegisterComponent {
  registerData = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  };
  
  isLoading = false;
  isBackendConnected = false; // Set to true when backend is connected

  constructor(private router: Router) {}

  isFormValid(): boolean {
    return !!(
      this.registerData.firstName &&
      this.registerData.lastName &&
      this.registerData.email &&
      this.registerData.password &&
      this.registerData.confirmPassword &&
      this.registerData.password === this.registerData.confirmPassword
    );
  }

  onSubmit(): void {
    if (!this.isFormValid()) {
      if (this.registerData.password !== this.registerData.confirmPassword) {
        alert('Passwords do not match!');
      }
      return;
    }

    this.isLoading = true;

    // Simulate API call
    setTimeout(() => {
      this.isLoading = false;
      
      if (this.isBackendConnected) {
        // Real registration logic will go here
        alert('Registration functionality will work when backend is connected!');      } else {
        // Demo mode
        alert(`Demo Registration Successful!\n\nAccount created for: ${this.registerData.email}\n\nRedirecting to login...`);
        this.router.navigate(['/login']);
      }
    }, 1000);
  }
}
