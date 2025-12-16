import { Component } from '@angular/core';
import { Subject, debounceTime } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { DocumentService } from '../../services/document-service';

@Component({
  imports: [FormsModule],
  selector: 'app-searchbar',
  templateUrl: './searchbar.html',
  styleUrls: ['./searchbar.css'],
  standalone: true
})

export class Searchbar {
  searchText: string = '';
  private searchSubject = new Subject<string>();

  constructor(private documentService: DocumentService) {
    this.searchSubject
      .pipe(/*debounceTime(300)*/)
      .subscribe(query => {
        if (!query || query.trim().length === 0) {
          console.log('Empty search');
          return;
        }

        this.documentService.searchDocuments(query).subscribe(results => { console.log('Elasticsearch results:', results); });
      });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchText);
  }
}
