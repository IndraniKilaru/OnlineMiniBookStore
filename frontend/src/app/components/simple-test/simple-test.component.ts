import { Component } from '@angular/core';

@Component({
  selector: 'app-simple-test',
  template: `
    <div style="padding: 20px; background: #f0f0f0; min-height: 100vh;">
      <h1>Simple Test Component Loading Successfully!</h1>
      <p>Router and app.component.html are working correctly.</p>
      <p>Navigation: <a routerLink="/test">Test Component</a> | <a routerLink="/home">Home Component</a></p>
    </div>
  `
})
export class SimpleTestComponent {
  constructor() {
    console.log('SimpleTestComponent loaded successfully!');
  }
}
