import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-pagination-component',
  imports: [],
  templateUrl: './pagination-component.html',
  styleUrl: './pagination-component.scss'
})
export class PaginationComponent {
  @Input() currentPage!: number; // Current page number
  @Input() totalPages!: number; // Total number of pages
  @Output() pageChange = new EventEmitter<number>(); // Event emitter for page changes

  constructor() {}

  // Emit the new page number when "Previous" is clicked
  previousPage() {
    if (this.currentPage > 1) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }

  // Emit the new page number when "Next" is clicked
  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.pageChange.emit(page);
    }
  }

  get pageRange(): (number | '...')[] {
    const range: (number | '...')[] = [];
    const maxVisiblePages = 5;
    const delta = Math.floor(maxVisiblePages / 2);

    const start = Math.max(1, this.currentPage - delta);
    const end = Math.min(this.totalPages, this.currentPage + delta);

    if (start > 1) {
      range.push(1);
      if (start > 2) range.push('...');
    }

    for (let i = start; i <= end; i++) {
      range.push(i);
    }

    if (end < this.totalPages) {
      if (end < this.totalPages - 1) range.push('...');
      range.push(this.totalPages);
    }

    return range;
  }

}
