import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

export interface DocumentItem {
  id: string;
  title: string;
  content: string;
  size: number;
  filetype: string;
  createdOn: string;
  modifiedLast: string; 
}

@Component({
  selector: 'app-document-list',
  templateUrl: './document-list.html',
  styleUrls: ['./document-list.css'],
  imports: [CommonModule]
})
export class DocumentList {
  /** Documents to display */
  @Input() documents: DocumentItem[] = [];

  /** Emit selected document id when clicked */
  @Output() selectDocument = new EventEmitter<DocumentItem>();



  /** Keep track of which document is selected */
  selectedId: string | null = null;

  onSelect(doc: DocumentItem): void {
    this.selectedId = doc.id;
    this.selectDocument.emit(doc);
  }
}
