import { Component } from '@angular/core';

@Component({
  selector: 'app-test-home',
  template: `
    <div style="padding: 2rem; text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; color: white;">
      <h1 style="font-size: 3rem; margin-bottom: 1rem;">🎉 Success! 🎉</h1>
      <h2 style="font-size: 2rem; margin-bottom: 2rem;">Mini Online Bookstore is Working!</h2>
      
      <div style="background: rgba(255,255,255,0.1); padding: 2rem; border-radius: 10px; margin: 2rem auto; max-width: 600px;">
        <h3>✅ Frontend Status</h3>
        <p style="font-size: 1.2rem;">Angular application is running successfully on port 4200</p>
        
        <div style="margin: 2rem 0;">
          <button onclick="alert('Navigation working!')" 
                  style="background: white; color: #667eea; border: none; padding: 1rem 2rem; border-radius: 5px; margin: 0.5rem; cursor: pointer; font-size: 1rem;">
            Test Button
          </button>
        </div>
        
        <div style="margin-top: 2rem;">
          <h4>Next Steps:</h4>
          <ol style="text-align: left; display: inline-block;">
            <li>Start backend services: <code style="background: rgba(0,0,0,0.3); padding: 0.2rem;">.\start-backend.ps1</code></li>
            <li>Test full application functionality</li>
            <li>Deploy to production</li>
          </ol>
        </div>
      </div>
      
      <div style="margin-top: 2rem; font-size: 0.9rem; opacity: 0.8;">
        <p>Built with Angular 12+ & ASP.NET Core</p>
        <p>Project Structure: ✅ Frontend | ⏳ Backend | 🔄 Integration</p>
      </div>
    </div>
  `
})
export class TestHomeComponent {
  constructor() {
    console.log('TestHomeComponent loaded successfully!');
  }
}
