import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../services/document-service';
import { DocumentItem } from '../../services/document-service';

@Component({
  imports: [CommonModule],
  selector: 'app-upload-document',
  templateUrl: './upload.html',
  styleUrls: ['./upload.css'],
  standalone: true
})

export class Upload{
  files: File[] = [];
  isUploading:boolean = false;
  completed:number = 0;

  @Output() uploadFinished = new EventEmitter<void>();

  constructor(private documentService: DocumentService) {}

  /**
   * Handle dragover event to allow dropping.
   * Prevent default so the browser does not open the file.
   */
  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
  }

  /**
   * Handle file drop
   */
  onDrop(event: DragEvent): void {
    event.preventDefault();
    if (event.dataTransfer?.files) {
      for (let i = 0; i < event.dataTransfer.files.length; i++) {
        this.files.push(event.dataTransfer.files[i]);
      }
    }
  }

  /**
   * Handle manual file selection via file input
   */
  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      for (let i = 0; i < input.files.length; i++) {
        this.files.push(input.files[i]);
      }
    }
  }

  /**
   * Simulated upload method.
   * Here you would usually send files to a backend via HttpClient.
   */
  uploadFiles(): void {
    if (!this.files || this.files.length === 0) {
      alert('No files selected!');
      return;
    }

    this.isUploading = true;
    this.completed = 0;

    const totalFiles = this.files.length;

    for (const file of this.files) {
      const formData = new FormData();
      formData.append('file', file, file.name);
      formData.append('id', '');
      formData.append('title', file.name);
      formData.append('summary', '');
      formData.append('fileSize', file.size.toString());
      formData.append('fileType', file.type);
      formData.append('createdOn', new Date().toISOString());
      formData.append('modifiedLast', new Date(file.lastModified).toISOString());

      this.documentService.createDocument(formData).subscribe({
        next: () => {
          this.completed++;
          if (this.completed === totalFiles) {
            this.finishUpload();
          }
        },
        error: () => {
          this.completed++;
          if (this.completed === totalFiles) {
            this.finishUpload();
          }
        }
      });
    }
  }

  private finishUpload() {
    this.isUploading = false;
    this.files = [];
    this.completed = 0;
    this.uploadFinished.emit();
  }

  removeFile(index: number): void {
    this.files.splice(index, 1);
  }
}