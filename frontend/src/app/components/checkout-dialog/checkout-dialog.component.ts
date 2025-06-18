import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface CheckoutDialogData {
  cartItems: any[];
  total: number;
}

@Component({
  selector: 'app-checkout-dialog',
  template: `
    <h2 mat-dialog-title>Confirm Order</h2>
    <mat-dialog-content>
      <p>Are you sure you want to place this order?</p>
      
      <div class="order-summary">
        <h3>Order Summary:</h3>
        <div *ngFor="let item of data.cartItems" class="item-summary">
          <span>{{item.book.title}} x{{item.quantity}}</span>
          <span>\${{(item.book.price * item.quantity).toFixed(2)}}</span>
        </div>
        <div class="total-line">
          <strong>Total: \${{data.total.toFixed(2)}}</strong>
        </div>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" (click)="onConfirm()">
        Place Order
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .order-summary {
      margin: 16px 0;
      padding: 16px;
      background: #f5f5f5;
      border-radius: 4px;
    }

    .order-summary h3 {
      margin-top: 0;
      margin-bottom: 12px;
    }

    .item-summary {
      display: flex;
      justify-content: space-between;
      margin-bottom: 8px;
    }

    .total-line {
      border-top: 1px solid #ddd;
      padding-top: 8px;
      margin-top: 12px;
      display: flex;
      justify-content: space-between;
    }
  `]
})
export class CheckoutDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<CheckoutDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CheckoutDialogData
  ) {}

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
