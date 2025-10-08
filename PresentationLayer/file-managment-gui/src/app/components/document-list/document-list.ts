import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { DocumentService } from '../../services/document-service';
import { DocumentItem } from '../../services/document-service';

/*export interface DocumentItem {
  Id: string;        // C#: Id
  Title: string;     // C#: Title
  Summary: string;   // C#: Summary
  FileSize: number;  // C#: FileSize
  FileType: string;  // C#: FileType
  CreatedOn: string; // C#: CreatedOn
  ModifiedLast: string; // C#: ModifiedLast
}*/

@Component({
  selector: 'app-document-list',
  templateUrl: './document-list.html',
  styleUrls: ['./document-list.css'],
  imports: [CommonModule]
})
export class DocumentList {
  /** Documents to display */
  
  constructor(private documentService: DocumentService) {}

  
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
