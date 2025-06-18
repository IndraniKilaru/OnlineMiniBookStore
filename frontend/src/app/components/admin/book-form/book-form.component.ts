import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BooksService } from '../../../services/books.service';
import { Book, CreateBookRequest, UpdateBookRequest } from '../../../models/book.model';

interface DialogData {
  mode: 'create' | 'edit';
  book?: Book;
}

@Component({
  selector: 'app-book-form',
  template: `
    <div class="book-form-container">
      <h2 mat-dialog-title>
        {{ data.mode === 'create' ? 'Add New Book' : 'Edit Book' }}
      </h2>
      
      <mat-dialog-content>
        <form [formGroup]="bookForm" class="book-form">
          <div class="form-row">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Title</mat-label>
              <input matInput formControlName="title" required>
              <mat-error *ngIf="bookForm.get('title')?.hasError('required')">
                Title is required
              </mat-error>
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Author</mat-label>
              <input matInput formControlName="author" required>
              <mat-error *ngIf="bookForm.get('author')?.hasError('required')">
                Author is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
              <mat-label>ISBN</mat-label>
              <input matInput formControlName="isbn" required>
              <mat-error *ngIf="bookForm.get('isbn')?.hasError('required')">
                ISBN is required
              </mat-error>
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Category</mat-label>
              <mat-select formControlName="category" required>
                <mat-option *ngFor="let category of categories" [value]="category">
                  {{category}}
                </mat-option>
              </mat-select>
              <mat-error *ngIf="bookForm.get('category')?.hasError('required')">
                Category is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Price</mat-label>
              <input matInput type="number" formControlName="price" min="0" step="0.01" required>
              <span matPrefix>$&nbsp;</span>
              <mat-error *ngIf="bookForm.get('price')?.hasError('required')">
                Price is required
              </mat-error>
              <mat-error *ngIf="bookForm.get('price')?.hasError('min')">
                Price must be positive
              </mat-error>
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Stock Quantity</mat-label>
              <input matInput type="number" formControlName="stockQuantity" min="0" required>
              <mat-error *ngIf="bookForm.get('stockQuantity')?.hasError('required')">
                Stock quantity is required
              </mat-error>
              <mat-error *ngIf="bookForm.get('stockQuantity')?.hasError('min')">
                Stock must be non-negative
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Image URL</mat-label>
              <input matInput formControlName="imageUrl" type="url">
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="4"></textarea>
            </mat-form-field>
          </div>
        </form>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" 
                [disabled]="bookForm.invalid || isLoading"
                (click)="onSubmit()">
          <mat-spinner diameter="20" *ngIf="isLoading"></mat-spinner>
          <span *ngIf="!isLoading">
            {{ data.mode === 'create' ? 'Add Book' : 'Update Book' }}
          </span>
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .book-form-container {
      min-width: 500px;
    }

    .book-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding-top: 16px;
    }

    .form-row {
      display: flex;
      gap: 16px;
      align-items: flex-start;
    }

    .full-width {
      width: 100%;
    }

    .half-width {
      width: calc(50% - 8px);
    }

    .mat-mdc-dialog-content {
      max-height: 70vh;
      overflow-y: auto;
    }

    .mat-mdc-dialog-actions {
      padding: 16px 24px;
    }
  `]
})
export class BookFormComponent implements OnInit {
  bookForm: FormGroup;
  isLoading = false;
  categories: string[] = [
    'Fiction',
    'Non-Fiction',
    'Science Fiction',
    'Fantasy',
    'Mystery',
    'Romance',
    'Thriller',
    'Biography',
    'History',
    'Self-Help',
    'Programming',
    'Business'
  ];

  constructor(
    private fb: FormBuilder,
    private booksService: BooksService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<BookFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    this.bookForm = this.fb.group({
      title: ['', Validators.required],
      author: ['', Validators.required],
      isbn: ['', Validators.required],
      category: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      imageUrl: [''],
      description: ['']
    });
  }

  ngOnInit() {
    this.loadCategories();
    
    if (this.data.mode === 'edit' && this.data.book) {
      this.bookForm.patchValue(this.data.book);
    }
  }

  loadCategories() {
    this.booksService.getCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data;
        }
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  onSubmit() {
    if (this.bookForm.valid) {
      this.isLoading = true;
      const formValue = this.bookForm.value;

      if (this.data.mode === 'create') {
        const createRequest: CreateBookRequest = formValue;
        this.booksService.createBook(createRequest).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.dialogRef.close(response.data);
            } else {
              this.snackBar.open('Error creating book', 'Close', { duration: 3000 });
            }
          },
          error: (error) => {
            this.isLoading = false;
            console.error('Error creating book:', error);
            this.snackBar.open('Error creating book', 'Close', { duration: 3000 });
          }
        });
      } else if (this.data.mode === 'edit' && this.data.book) {
        const updateRequest: UpdateBookRequest = formValue;
        this.booksService.updateBook(this.data.book.id, updateRequest).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.success) {
              this.dialogRef.close(response.data);
            } else {
              this.snackBar.open('Error updating book', 'Close', { duration: 3000 });
            }
          },
          error: (error) => {
            this.isLoading = false;
            console.error('Error updating book:', error);
            this.snackBar.open('Error updating book', 'Close', { duration: 3000 });
          }
        });
      }
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
