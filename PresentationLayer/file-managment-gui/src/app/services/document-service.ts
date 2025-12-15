import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DocumentItem {
  id: string;
  title: string;
  summary: string;
  fileSize: number;
  fileType: string;
  createdOn: string;
  modifiedLast: string;
}

@Injectable({ providedIn: 'root' })

export class DocumentService {
  private apiUrl = 'api/MetaData'; // Adjust port if needed

  constructor(private http: HttpClient) {}

  /** Get all documents */
  getDocuments(): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(this.apiUrl);
  }

  /** Get single document by Guid */
  getDocument(id: string): Observable<DocumentItem> {
    return this.http.get<DocumentItem>(`${this.apiUrl}/${id}`);
  }

  /** Create new document (metadata + file upload) */
  createDocument(formData: FormData): Observable<DocumentItem> {
    return this.http.post<DocumentItem>(`${this.apiUrl}/upload`, formData);
  }

  /** Update an existing document on the server */
  updateDocument(document: DocumentItem): Observable<DocumentItem> {
    const url = `${this.apiUrl}/${document.id}`;
    return this.http.put<DocumentItem>(url, document);
  }

  deleteDocument(id: string): Observable<DocumentItem> {
    return this.http.delete<DocumentItem>(`${this.apiUrl}/${id}`);
  }

  /** Search documents via Elasticsearch */
  searchDocuments(query: string): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(`${this.apiUrl}/search`, { params: { q: query } });
  }
}