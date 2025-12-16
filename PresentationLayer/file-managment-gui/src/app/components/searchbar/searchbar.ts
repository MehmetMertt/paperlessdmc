import { Component, EventEmitter, Output } from '@angular/core';
import { Subject, debounceTime } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { FormsModule } from '@angular/forms';
import { DocumentService } from '../../services/document-service';
import { DocumentItem } from '../../services/document-service';

@Component({
  imports : [FormsModule],
  selector: 'app-searchbar',
  templateUrl: './searchbar.html',
  styleUrls: ['./searchbar.css'],
  standalone : true
})
export class Searchbar {
  // Two-way bound search text
  searchText: string = '';

  // Emits search queries to parent components
  @Output() searchResult = new EventEmitter<DocumentItem[]>();

  // Subject to handle debounce timing
  private searchSubject = new Subject<string>();


    constructor(private documentService: DocumentService) {
    // Set up the debounce + API call pipeline
    this.searchSubject.pipe(
      debounceTime(1000),            // Wait 300ms after last keystroke
      switchMap((query: string) => this.documentService.searchDocument(query)) // Call API
    ).subscribe({
      next: (docs: DocumentItem[]) => {
        console.log('Search results:', docs);
        this.searchResult.emit(docs); // Emit results to parent
      },
      error: (err) => {
        console.error('Search failed:', err);
        this.searchResult.emit([]); // fallback empty array
      }
    });
  }

  /** Called on each input change */
  onSearchChange(): void {
    this.searchSubject.next(this.searchText);
  }
  /**
   * Called on each input change.
   * Push the value into the Subject to trigger debounce.
   
  onSearchChange(): void {
    this.searchSubject.next(this.searchText);
    var docs: DocumentItem[] = [];


      this.documentService.searchDocument(this.searchText).subscribe({
        next: (docs) => {
          console.log(`Documents "${docs}" were found!`);
          this.searchResult.emit(docs); // <-- emit the actual array

        },
        error: (err) => {
          console.error('Failed to find any documents:', err);
          console.log('Failed to find any documents:');

          alert('Failed find this document. Check the console for details.');
        },
      });



  }*/
}

