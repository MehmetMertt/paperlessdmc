import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DocumentList} from './components/document-list/document-list';
import { Searchbar } from './components/searchbar/searchbar';
import { DocumentDetail } from './components/document-detail/document-detail';
import { Upload } from './components/upload/upload';
//import { DocumentItem } from './components/document-list/document-list';
import { DocumentItem } from './services/document-service';
import { DocumentService } from './services/document-service';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet,DocumentList, Searchbar, DocumentDetail,Upload],
  templateUrl: './app.html',
  styleUrl: './app.css',
})

export class App {
  constructor(private documentService: DocumentService) {}
  documents: DocumentItem[] = [];

  /** Fetch documents from API */
  loadDocuments() {
    this.documentService.getDocuments().subscribe({
      next: docs => {
        if (!docs || docs.length === 0) {
          // kein Ergebnis: wir erzeugen ein Mock-Dokument
          this.documents = [{
            id: '00000000-0000-0000-0000-000000000000',
            title: 'No documents found',
            summary: 'This is a mock document because no documents exist in the database.',
            fileSize: 0,
            fileType: 'none',
            createdOn: new Date().toISOString(),
            modifiedLast: new Date().toISOString()
          }];
        } else {
          this.documents = docs;
        }
      },
      error: err => {
        console.error('Error loading documents', err);
        // auch hier: Mock-Dokument anzeigen
        this.documents = [{
          id: '00000000-0000-0000-0000-000000000000',
          title: 'Error loading documents',
          summary: 'This is a mock document because the API call failed.',
          fileSize: 0,
          fileType: 'none',
          createdOn: new Date().toISOString(),
          modifiedLast: new Date().toISOString()
        }];
      }
    });
  }

  ngOnInit(){
    this.loadDocuments();
  }

  onDeleteDocument() {
    this.loadDocuments();
  }

  onUpdateDocument() {
    this.loadDocuments();
  }

  selectedDocument: DocumentItem | null = null;

  onDocumentSelected(doc: DocumentItem) {
    console.log('Parent received selected doc:', doc);
    // Do whatever you need: open, load, etc.
    this.selectedDocument = doc;
  }

  onDocumentUploaded(doc: DocumentItem) {
    console.log('App received uploaded doc:', doc);

    // adds new document to list
    this.documents = [doc, ...this.documents];
  }
}