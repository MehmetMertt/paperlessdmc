import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DocumentList} from './components/document-list/document-list';
import { Searchbar } from './components/searchbar/searchbar';
import { DocumentDetail } from './components/document-detail/document-detail';
import { Upload } from './components/upload/upload';
import { DocumentItem } from './components/document-list/document-list';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet,DocumentList, Searchbar, DocumentDetail,Upload],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App {
    // Generate 20 mock documents
  mockDocuments: DocumentItem[] = this.generateMockDocuments(20);

  // The filtered list passed to the document-list component
  filteredDocuments: DocumentItem[] = [...this.mockDocuments];

  generateMockDocuments(count: number): DocumentItem[] {
    const lorem = `Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
    Integer posuere erat a ante. Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor.`;
    const docs: DocumentItem[] = [];
    for (let i = 1; i <= count; i++) {
      docs.push({
        id: `doc-${i}`,
        title: `Document ${i} — Sample Title`,
        content: `${lorem} (document ${i} content — paragraph 1)\n\n${lorem} (document ${i} content — paragraph 2)`,
        size: 10000,
        filetype: "some File",
        createdOn: "25.09.2025",
        modifiedLast: "26.09.2025",
      });
    }
    return docs;
  }

  selectedDocument: DocumentItem | null = null;

onDocumentSelected(doc: DocumentItem) {
    console.log('Parent received selected doc:', doc);
    // Do whatever you need: open, load, etc.
    this.selectedDocument = doc;
  }
}


