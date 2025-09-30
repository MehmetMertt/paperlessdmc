import { Component, Input } from '@angular/core';
import { DocumentItem } from '../document-list/document-list';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-document-detail',
  templateUrl: './document-detail.html',
  styleUrls: ['./document-detail.css'],
  imports: [CommonModule]
})
export class DocumentDetail {
  /** Selected document to display */
  @Input() document? : DocumentItem | null = null;
}
