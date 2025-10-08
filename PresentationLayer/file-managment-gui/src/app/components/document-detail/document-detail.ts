import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DocumentItem } from '../../services/document-service';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../services/document-service';
@Component({
  selector: 'app-document-detail',
  templateUrl: './document-detail.html',
  styleUrls: ['./document-detail.css'],
  imports: [CommonModule]
})
export class DocumentDetail {
  /** Selected document to display */
  @Input() document?: DocumentItem | null = null;

  /** Events for parent component */
  @Output() delete = new EventEmitter<string>();
  @Output() update = new EventEmitter<string>();
  
  constructor(private documentService: DocumentService) {}



  onDelete(): void {
  if (!this.document) return;

    if (confirm(`Are you sure you want to delete "${this.document.title}"?`)) {
      this.documentService.deleteDocument(this.document.id).subscribe({
        next: () => {
          alert(`Document "${this.document?.title}" was deleted!`);
          console.log('Deleted document:', this.document);

          // notify parent component so it can refresh its list
          this.delete.emit(this.document?.id);
        },
        error: (err) => {
          console.error('Failed to delete document:', err);
          console.log('Document:', this.document);

          alert('Failed to delete this document. Check the console for details.');
        },
      });
    }
  }

  onUpdate(): void {
    if (this.document) {
      this.update.emit(this.document.id);
    }
  }
}
