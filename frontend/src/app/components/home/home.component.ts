import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  template: `
    <div class="home-container">
      <div class="hero-section">
        <h1>Welcome to Mini Online Bookstore! 📚</h1>
        <p class="hero-subtitle">Discover amazing books and start your reading journey</p>
        
        <div class="hero-actions" *ngIf="!authService.isAuthenticated()">
          <a routerLink="/register" class="btn btn-primary">Get Started</a>
          <a routerLink="/login" class="btn btn-secondary">Sign In</a>
        </div>
        
        <div class="hero-actions" *ngIf="authService.isAuthenticated()">
          <a routerLink="/books" class="btn btn-primary">Browse Books</a>
          <a routerLink="/cart" class="btn btn-secondary">View Cart</a>
        </div>
      </div>

      <div class="features-section">
        <h2>Why Choose Our Bookstore?</h2>
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">📖</div>
            <h3>Vast Collection</h3>
            <p>Thousands of books across all genres</p>
          </div>
          
          <div class="feature-card">
            <div class="feature-icon">🚚</div>
            <h3>Fast Delivery</h3>
            <p>Quick and reliable shipping</p>
          </div>
          
          <div class="feature-card">
            <div class="feature-icon">💰</div>
            <h3>Best Prices</h3>
            <p>Competitive prices with great deals</p>
          </div>
          
          <div class="feature-card">
            <div class="feature-icon">🔒</div>
            <h3>Secure Shopping</h3>
            <p>Safe and secure payment process</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      max-width: 1200px;
      margin: 0 auto;
    }

    .hero-section {
      text-align: center;
      padding: 4rem 2rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-radius: 10px;
      margin-bottom: 3rem;
    }

    .hero-section h1 {
      font-size: 3rem;
      margin-bottom: 1rem;
      font-weight: 700;
    }

    .hero-subtitle {
      font-size: 1.2rem;
      margin-bottom: 2rem;
      opacity: 0.9;
    }

    .hero-actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;
    }

    .btn {
      padding: 1rem 2rem;
      border: none;
      border-radius: 5px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.3s ease;
      cursor: pointer;
    }

    .btn-primary {
      background: white;
      color: #667eea;
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
    }

    .btn-secondary {
      background: transparent;
      color: white;
      border: 2px solid white;
    }

    .btn-secondary:hover {
      background: white;
      color: #667eea;
    }

    .features-section {
      padding: 2rem;
    }

    .features-section h2 {
      text-align: center;
      margin-bottom: 3rem;
      color: #333;
      font-size: 2.5rem;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 2rem;
    }

    .feature-card {
      background: white;
      padding: 2rem;
      border-radius: 10px;
      text-align: center;
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
      transition: transform 0.3s ease;
    }

    .feature-card:hover {
      transform: translateY(-5px);
    }

    .feature-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .feature-card h3 {
      color: #333;
      margin-bottom: 1rem;
    }

    .feature-card p {
      color: #666;
      line-height: 1.6;
    }

    @media (max-width: 768px) {
      .hero-section h1 {
        font-size: 2rem;
      }
      
      .hero-actions {
        flex-direction: column;
        align-items: center;
      }
      
      .btn {
        width: 200px;
      }
    }
  `]
})
export class HomeComponent {
  constructor(public authService: AuthService) {}
}
