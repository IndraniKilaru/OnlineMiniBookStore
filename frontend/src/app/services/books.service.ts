import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Book, CreateBookRequest, UpdateBookRequest } from '../models/book.model';
import { ApiResponse, PagedResult } from '../models/api.model';

@Injectable({
  providedIn: 'root'
})
export class BooksService {
  private readonly baseUrl = `${environment.apiUrl}/books`;

  constructor(private http: HttpClient) {}

  getBooks(page: number = 1, pageSize: number = 10, category?: string, searchTerm?: string): Observable<ApiResponse<PagedResult<Book>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (category) {
      params = params.set('category', category);
    }

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<ApiResponse<PagedResult<Book>>>(this.baseUrl, { params });
  }

  getBook(id: number): Observable<ApiResponse<Book>> {
    return this.http.get<ApiResponse<Book>>(`${this.baseUrl}/${id}`);
  }

  createBook(book: CreateBookRequest): Observable<ApiResponse<Book>> {
    return this.http.post<ApiResponse<Book>>(this.baseUrl, book);
  }

  updateBook(id: number, book: UpdateBookRequest): Observable<ApiResponse<Book>> {
    return this.http.put<ApiResponse<Book>>(`${this.baseUrl}/${id}`, book);
  }

  deleteBook(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  getCategories(): Observable<ApiResponse<string[]>> {
    return this.http.get<ApiResponse<string[]>>(`${this.baseUrl}/categories`);
  }
}
