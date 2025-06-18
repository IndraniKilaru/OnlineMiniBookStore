export interface Book {
  id: number;
  title: string;
  author: string;
  isbn: string;
  price: number;
  description: string;
  category: string;
  stockQuantity: number;
  imageUrl: string;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  isbn: string;
  price: number;
  description: string;
  category: string;
  stockQuantity: number;
  imageUrl: string;
}

export interface UpdateBookRequest {
  title?: string;
  author?: string;
  isbn?: string;
  price?: number;
  description?: string;
  category?: string;
  stockQuantity?: number;
  imageUrl?: string;
}
