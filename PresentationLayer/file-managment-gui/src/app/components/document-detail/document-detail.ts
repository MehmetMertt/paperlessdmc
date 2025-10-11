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

  /*onUpdate(): void {
    if (this.document) {
      this.update.emit(this.document.id);
    }
  }*/


 onUpdate(): void {
    if (!this.document) return;

    // 1️⃣ Ask user for a new document name
    const newName = prompt(
      `Enter a new name for "${this.document.title}".\n` +
      `The "Modified Last"- date will be updated.`
    );
    this.update.emit(this.document.id);


    // 2️⃣ If user cancelled or didn't enter anything, stop
    if (!newName || newName.trim() === '') {
      alert('Update cancelled — no new name entered.');
      return;
    }

      // 3️⃣ Create updated document object

      let updatedDocument: DocumentItem = {
      ...this.document,
      title: newName.trim(),
      modifiedLast: new Date().toISOString(),


    };

    // 4️⃣ Call the service to update it
    this.documentService.updateDocument(updatedDocument).subscribe({
      next: () => {
        alert(`Document updated successfully!\nNew name: "${updatedDocument.title}"`);
        console.log('Updated document:', updatedDocument);

        // Update local display immediately
        this.document = updatedDocument;

        // Notify parent so it can refresh list
        this.update.emit(updatedDocument.id);
      },
      error: (err) => {
        console.error('Failed to update document:', err);
        alert('Failed to update the document. Check console for details.');
      },
    });


  }



  

}
