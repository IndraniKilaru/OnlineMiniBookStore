import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-simple-home',
  template: `
    <div class="home-container">
      <div class="hero-section">
        <h1>🎉 Welcome to Mini Online Bookstore! 📚</h1>
        <p class="hero-subtitle">Your Angular + ASP.NET Core application is running successfully!</p>
        
        <div class="hero-actions">
          <button (click)="goToBooks()" class="btn btn-primary">Browse Books</button>
          <button (click)="goToLogin()" class="btn btn-secondary">Login</button>
        </div>
      </div>

      <div class="status-section">
        <h2>✅ System Status</h2>
        <div class="status-grid">
          <div class="status-card success">
            <div class="status-icon">✅</div>
            <h3>Frontend</h3>
            <p>Angular application running on port 4200</p>
          </div>
          
          <div class="status-card warning">
            <div class="status-icon">⚠️</div>
            <h3>Backend</h3>
            <p>Start backend services to enable full functionality</p>
          </div>
          
          <div class="status-card info">
            <div class="status-icon">ℹ️</div>
            <h3>Database</h3>
            <p>SQL Server + Entity Framework ready</p>
          </div>
          
          <div class="status-card info">
            <div class="status-icon">🔧</div>
            <h3>Services</h3>
            <p>Microservices architecture implemented</p>
          </div>
        </div>
      </div>

      <div class="next-steps">
        <h2>🚀 Next Steps</h2>
        <ol>
          <li>Start the backend services using <code>start-backend.ps1</code></li>
          <li>Open <a href="https://localhost:5001/swagger" target="_blank">https://localhost:5001/swagger</a> to test APIs</li>
          <li>Register a new user or login with admin credentials</li>
          <li>Test the full application functionality</li>
        </ol>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .hero-section {
      text-align: center;
      padding: 4rem 2rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-radius: 15px;
      margin-bottom: 3rem;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    }

    .hero-section h1 {
      font-size: 3rem;
      margin-bottom: 1rem;
      font-weight: 700;
    }

    .hero-subtitle {
      font-size: 1.3rem;
      margin-bottom: 2rem;
      opacity: 0.95;
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
      border-radius: 8px;
      font-weight: 600;
      font-size: 1rem;
      cursor: pointer;
      transition: all 0.3s ease;
      min-width: 150px;
    }

    .btn-primary {
      background: white;
      color: #667eea;
    }

    .btn-primary:hover {
      transform: translateY(-3px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
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

    .status-section, .next-steps {
      margin-bottom: 3rem;
    }

    .status-section h2, .next-steps h2 {
      text-align: center;
      margin-bottom: 2rem;
      color: #333;
      font-size: 2.2rem;
    }

    .status-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }

    .status-card {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      text-align: center;
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
      border-left: 5px solid;
    }

    .status-card.success { border-left-color: #28a745; }
    .status-card.warning { border-left-color: #ffc107; }
    .status-card.info { border-left-color: #17a2b8; }

    .status-icon {
      font-size: 2.5rem;
      margin-bottom: 1rem;
    }

    .status-card h3 {
      color: #333;
      margin-bottom: 0.5rem;
    }

    .status-card p {
      color: #666;
      line-height: 1.5;
    }

    .next-steps {
      background: #f8f9fa;
      padding: 2rem;
      border-radius: 12px;
    }

    .next-steps ol {
      text-align: left;
      max-width: 600px;
      margin: 0 auto;
      font-size: 1.1rem;
      line-height: 1.8;
    }

    .next-steps li {
      margin-bottom: 0.5rem;
      color: #333;
    }

    .next-steps code {
      background: #e9ecef;
      padding: 0.2rem 0.5rem;
      border-radius: 4px;
      font-family: 'Courier New', monospace;
    }

    .next-steps a {
      color: #667eea;
      text-decoration: none;
      font-weight: 600;
    }

    .next-steps a:hover {
      text-decoration: underline;
    }

    @media (max-width: 768px) {
      .hero-section h1 {
        font-size: 2.2rem;
      }
      
      .hero-actions {
        flex-direction: column;
        align-items: center;
      }
      
      .btn {
        width: 200px;
      }

      .home-container {
        padding: 1rem;
      }
    }
  `]
})
export class SimpleHomeComponent {
  constructor(private router: Router) {}

  goToBooks(): void {
    this.router.navigate(['/books']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
