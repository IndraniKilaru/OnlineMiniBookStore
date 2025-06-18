import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BooksService } from '../../../services/books.service';
import { AuthService } from '../../../services/auth.service';
import { Book } from '../../../models/book.model';
import { BookFormComponent } from '../book-form/book-form.component';

@Component({
  selector: 'app-admin-dashboard',
  template: `
    <div class="admin-container">
      <div class="admin-header">
        <h1>Admin Dashboard</h1>
        <p>Welcome, {{userDisplayName}}! Manage your bookstore from here.</p>
      </div>
      
      <div class="admin-sections">
        <mat-card class="admin-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>book</mat-icon>
              Book Management
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>Add, edit, and remove books from the catalog</p>
            <div class="stats">
              <span class="stat-item">Total Books: {{totalBooks}}</span>
            </div>
          </mat-card-content>
          <mat-card-actions>
            <button mat-raised-button color="primary" (click)="openAddBookDialog()">
              <mat-icon>add</mat-icon>
              Add New Book
            </button>
            <button mat-button routerLink="/books">
              <mat-icon>view_list</mat-icon>
              View All Books
            </button>
          </mat-card-actions>
        </mat-card>

        <mat-card class="admin-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>people</mat-icon>
              User Management
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>Create admin users and manage permissions</p>
          </mat-card-content>
          <mat-card-actions>
            <button mat-raised-button color="accent" (click)="openCreateAdminDialog()">
              <mat-icon>person_add</mat-icon>
              Create Admin User
            </button>
          </mat-card-actions>
        </mat-card>

        <mat-card class="admin-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>receipt</mat-icon>
              Orders
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>View and manage customer orders</p>
          </mat-card-content>
          <mat-card-actions>
            <button mat-raised-button color="primary">
              <mat-icon>list</mat-icon>
              View Orders
            </button>
          </mat-card-actions>
        </mat-card>

        <mat-card class="admin-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>analytics</mat-icon>
              Analytics
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>View sales and performance metrics</p>
          </mat-card-content>
          <mat-card-actions>
            <button mat-raised-button color="primary">
              <mat-icon>bar_chart</mat-icon>
              View Analytics
            </button>
          </mat-card-actions>
        </mat-card>
      </div>

      <!-- Recent Books Table -->
      <mat-card class="recent-books-card">
        <mat-card-header>
          <mat-card-title>Recent Books</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <table mat-table [dataSource]="recentBooks" class="full-width">
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Title</th>
              <td mat-cell *matCellDef="let book">{{book.title}}</td>
            </ng-container>

            <ng-container matColumnDef="author">
              <th mat-header-cell *matHeaderCellDef>Author</th>
              <td mat-cell *matCellDef="let book">{{book.author}}</td>
            </ng-container>

            <ng-container matColumnDef="category">
              <th mat-header-cell *matHeaderCellDef>Category</th>
              <td mat-cell *matCellDef="let book">{{book.category}}</td>
            </ng-container>

            <ng-container matColumnDef="price">
              <th mat-header-cell *matHeaderCellDef>Price</th>
              <td mat-cell *matCellDef="let book">\${{book.price}}</td>
            </ng-container>

            <ng-container matColumnDef="stock">
              <th mat-header-cell *matHeaderCellDef>Stock</th>
              <td mat-cell *matCellDef="let book">{{book.stockQuantity}}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let book">
                <button mat-icon-button (click)="editBook(book)">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="deleteBook(book)">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .admin-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .admin-header {
      text-align: center;
      margin-bottom: 32px;
    }

    .admin-header h1 {
      color: #1976d2;
      margin-bottom: 8px;
    }

    .admin-sections {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 20px;
      margin-bottom: 32px;
    }

    .admin-card {
      transition: transform 0.2s;
    }

    .admin-card:hover {
      transform: translateY(-4px);
    }

    .admin-card mat-card-title {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .stats {
      margin-top: 16px;
      padding: 12px;
      background-color: #f5f5f5;
      border-radius: 4px;
    }

    .stat-item {
      font-weight: 500;
      color: #1976d2;
    }

    .recent-books-card {
      margin-top: 24px;
    }

    .full-width {
      width: 100%;
    }

    table {
      margin-top: 16px;
    }
  `]
})
export class AdminDashboardComponent implements OnInit {
  userDisplayName = '';
  totalBooks = 0;
  recentBooks: Book[] = [];
  displayedColumns: string[] = ['title', 'author', 'category', 'price', 'stock', 'actions'];

  constructor(
    private booksService: BooksService,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.userDisplayName = user ? `${user.firstName} ${user.lastName}` : '';
    });

    this.loadRecentBooks();
  }

  loadRecentBooks() {
    this.booksService.getBooks(1, 10).subscribe({
      next: (response) => {        if (response.success) {
          this.recentBooks = response.data.items;
          this.totalBooks = response.data.totalCount;
        }
      },
      error: (error) => {
        console.error('Error loading books:', error);
        this.snackBar.open('Error loading books', 'Close', { duration: 3000 });
      }
    });
  }

  openAddBookDialog() {
    const dialogRef = this.dialog.open(BookFormComponent, {
      width: '600px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadRecentBooks();
        this.snackBar.open('Book added successfully', 'Close', { duration: 3000 });
      }
    });
  }

  editBook(book: Book) {
    const dialogRef = this.dialog.open(BookFormComponent, {
      width: '600px',
      data: { mode: 'edit', book }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadRecentBooks();
        this.snackBar.open('Book updated successfully', 'Close', { duration: 3000 });
      }
    });
  }

  deleteBook(book: Book) {
    if (confirm(`Are you sure you want to delete "${book.title}"?`)) {
      this.booksService.deleteBook(book.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadRecentBooks();
            this.snackBar.open('Book deleted successfully', 'Close', { duration: 3000 });
          }
        },
        error: (error) => {
          console.error('Error deleting book:', error);
          this.snackBar.open('Error deleting book', 'Close', { duration: 3000 });
        }
      });
    }
  }

  openCreateAdminDialog() {
    // TODO: Implement admin creation dialog
    this.snackBar.open('Admin creation feature coming soon!', 'Close', { duration: 3000 });
  }
}
