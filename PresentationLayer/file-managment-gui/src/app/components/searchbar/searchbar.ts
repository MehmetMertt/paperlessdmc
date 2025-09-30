import { Component, EventEmitter, Output } from '@angular/core';
import { Subject, debounceTime } from 'rxjs';
import { FormsModule } from '@angular/forms';

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
  @Output() search = new EventEmitter<string>();

  // Subject to handle debounce timing
  private searchSubject = new Subject<string>();

  constructor() {
    // Apply debounce (300 ms) before emitting the search value
    this.searchSubject.pipe(
      debounceTime(300)
    ).subscribe((query) => {
      this.search.emit(query);
    });
  }

  /**
   * Called on each input change.
   * Push the value into the Subject to trigger debounce.
   */
  onSearchChange(): void {
    this.searchSubject.next(this.searchText);
  }
}
