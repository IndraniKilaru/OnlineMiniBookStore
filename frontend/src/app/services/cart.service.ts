import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api.model';

export interface CartItem {
  id: number;
  bookId: number;
  quantity: number;
  book: {
    id: number;
    title: string;
    author: string;
    price: number;
    imageUrl: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly baseUrl = `${environment.apiUrl}/cart`;
  private cartItemCountSubject = new BehaviorSubject<number>(0);
  
  public cartItemCount$ = this.cartItemCountSubject.asObservable();  constructor(private http: HttpClient) {
    this.loadCartItemCount();
  }
  private getCurrentUserId(): number {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      return user.id;
    }
    return 1; // Default to user ID 1 if not logged in
  }

  getCart(): Observable<ApiResponse<any>> {
    const userId = this.getCurrentUserId();
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/${userId}`)
      .pipe(
        tap(response => {
          if (response.success && response.data.items) {
            const totalItems = response.data.items.reduce((sum: number, item: any) => sum + item.quantity, 0);
            this.cartItemCountSubject.next(totalItems);
          }
        })
      );
  }

  addToCart(bookId: number, quantity: number = 1): Observable<ApiResponse<CartItem>> {
    const userId = this.getCurrentUserId();
    return this.http.post<ApiResponse<CartItem>>(`${this.baseUrl}/${userId}/items`, { bookId, quantity })
      .pipe(
        tap(() => {
          this.loadCartItemCount();
        })
      );
  }
  updateCartItem(id: number, quantity: number): Observable<ApiResponse<CartItem>> {
    const userId = this.getCurrentUserId();
    return this.http.put<ApiResponse<CartItem>>(`${this.baseUrl}/${userId}/items/${id}`, { quantity })
      .pipe(
        tap(() => {
          this.loadCartItemCount();
        })
      );
  }

  removeFromCart(id: number): Observable<ApiResponse<boolean>> {
    const userId = this.getCurrentUserId();
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${userId}/items/${id}`)
      .pipe(
        tap(() => {
          this.loadCartItemCount();
        })
      );
  }
  clearCart(): Observable<ApiResponse<boolean>> {
    const userId = this.getCurrentUserId();
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${userId}`)
      .pipe(
        tap(() => {
          this.cartItemCountSubject.next(0);
        })
      );
  }
  private loadCartItemCount() {
    this.getCart().subscribe({
      next: (response) => {
        if (response.success && response.data.items) {
          const totalItems = response.data.items.reduce((sum: number, item: any) => sum + item.quantity, 0);
          this.cartItemCountSubject.next(totalItems);
        }
      },
      error: () => {
        this.cartItemCountSubject.next(0);
      }
    });
  }
}
