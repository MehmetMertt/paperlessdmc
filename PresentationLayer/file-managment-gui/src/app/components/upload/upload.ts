import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  imports: [CommonModule],
  selector: 'app-upload-document',
  templateUrl: './upload.html',
  styleUrls: ['./upload.css']
})
export class Upload{
  // Store selected files
  files: File[] = [];

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
    if (this.files.length === 0) {
      alert('No files selected!');
      return;
    }

    // Replace this with a real API call (HttpClient.post)
    console.log('Uploading files:', this.files);
    alert(`${this.files.length} file(s) uploaded successfully!`);

    // Clear after upload
    this.files = [];
  }

  /**
   * Remove a file before upload (optional feature)
   */
  removeFile(index: number): void {
    this.files.splice(index, 1);
  }
}
