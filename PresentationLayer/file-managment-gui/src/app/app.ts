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
    this.documentService.getDocuments().subscribe(docs => { 
      this.documents = docs; 
      if(this.documents === null) return;
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