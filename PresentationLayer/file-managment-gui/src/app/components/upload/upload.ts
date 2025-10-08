import { Component } from '@angular/core';
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
  // Store selected files
  files: File[] = [];
  constructor(private documentService: DocumentService) {}

  isUploading: boolean = false; //check if upload is complete
  completed: number =0;

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

    console.log('Uploading files:', this.files);
    
    
    
    
  {/* Without sending file in request
  for (const file of this.files) {
    const metadata: Partial<DocumentItem> = {
      id: '',
      title: file.name,
      summary: '',
      fileSize: file.size,
      fileType: file.type,
      // createdOn cannot be extracted from the File object reliably
      createdOn: new Date().toISOString(),
      modifiedLast: new Date(file.lastModified).toISOString()
    };

    //this.documentService.createDocument(metadata).subscribe({
    // next: (res) => console.log('Uploaded successfully:', res) ,
    //  error: (err) => console.error('Error uploading metadata:', err)
    //});

    this.documentService.createDocument(metadata).subscribe({
      next: (res) => {
        console.log('Uploaded:', res);
        this.completed++;
        if (this.completed === this.files.length) {
          this.isUploading = false;
          alert(`${this.files.length} file(s) metadata sent successfully!`);
          // Clear after upload
          this.files = [];


        }
      },
      error: (err) => {
        console.error('Upload error:', err);
        this.completed++;
        if (this.completed === this.files.length) {
          this.isUploading = false;
        }
      }
    });


  }*/}


  for (const file of this.files) {
    const formData = new FormData();

    // Append file itself
    formData.append('file', file, file.name);

    // Append metadata fields
    formData.append('id', ''); // empty id as requested
    formData.append('title', file.name);
    formData.append('summary', '');
    formData.append('fileSize', file.size.toString());
    formData.append('fileType', file.type);
    formData.append('createdOn', new Date().toISOString());
    formData.append('modifiedLast', new Date(file.lastModified).toISOString());

    this.documentService.createDocument(formData).subscribe({
      next: (res) => {
        console.log('File + metadata uploaded:', res);
        this.completed++;
        if (this.completed === this.files.length) {
          this.isUploading = false;
          alert(`${this.files.length} file(s) uploaded successfully!`);
          this.files = [];
        }
      },
      error: (err) => {
        console.error('Upload error:', err);
        this.completed++;
        if (this.completed === this.files.length) {
          this.isUploading = false;
        }
      }
    });
  }

  this.isUploading = false;
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
