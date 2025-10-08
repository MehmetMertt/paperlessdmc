import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DocumentItem } from '../../services/document-service';
import { CommonModule } from '@angular/common';

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

  onDelete(): void {
    if (this.document) {
      this.delete.emit(this.document.Id);
    }
  }

  onUpdate(): void {
    if (this.document) {
      this.update.emit(this.document.Id);
    }
  }
}
