import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api.model';

export interface Order {
  id: number;
  userId: number;
  status: 'Pending' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
  orderItems: OrderItem[];
}

export interface OrderItem {
  id: number;
  bookId: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  book?: {
    id: number;
    title: string;
    author: string;
    price: number;
    imageUrl: string;
  };
}

export interface CreateOrderRequest {
  items: Array<{
    bookId: number;
    quantity: number;
  }>;
}

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private readonly baseUrl = `${environment.apiUrl}/orders`;

  constructor(private http: HttpClient) {}

  private getCurrentUserId(): number {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      return user.id;
    }
    return 1; // Default to user ID 1 if not logged in
  }

  createOrder(orderRequest: CreateOrderRequest): Observable<ApiResponse<Order>> {
    const userId = this.getCurrentUserId();
    return this.http.post<ApiResponse<Order>>(`${this.baseUrl}/${userId}`, orderRequest);
  }

  getUserOrders(): Observable<ApiResponse<Order[]>> {
    const userId = this.getCurrentUserId();
    return this.http.get<ApiResponse<Order[]>>(`${this.baseUrl}/user/${userId}`);
  }

  getOrder(orderId: number): Observable<ApiResponse<Order>> {
    return this.http.get<ApiResponse<Order>>(`${this.baseUrl}/${orderId}`);
  }

  getAllOrders(): Observable<ApiResponse<Order[]>> {
    return this.http.get<ApiResponse<Order[]>>(`${this.baseUrl}`);
  }
}
