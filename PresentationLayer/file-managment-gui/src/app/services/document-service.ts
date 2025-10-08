import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';




// Match your C# API property names
export interface DocumentItem {
  id: string;
  title: string;
  summary: string;
  fileSize: number;
  fileType: string;
  createdOn: string;
  modifiedLast: string;
}

@Injectable({
  providedIn: 'root'
})


export class DocumentService {
  private apiUrl = 'https://localhost:7212/api/MetaData'; // Adjust port if needed


  
  constructor(private http: HttpClient) {}

  /** Get all documents */
  getDocuments(): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(this.apiUrl);
  }

  /** Get single document by Guid */
  getDocument(id: string): Observable<DocumentItem> {
    return this.http.get<DocumentItem>(`${this.apiUrl}/${id}`);
  }

  /** Create new document (using your POST endpoint) */
  /*createDocument(doc: Partial<DocumentItem>): Observable<DocumentItem> {
    return this.http.post<DocumentItem>(this.apiUrl, doc);
  }*/

  /** Create new document (metadata + file upload) */
  createDocument(formData: FormData): Observable<DocumentItem> {
  return this.http.post<DocumentItem>(this.apiUrl, formData);
}


  deleteDocument(id: string): Observable<DocumentItem> {
    return this.http.delete<DocumentItem>(`${this.apiUrl}/${id}`);
  }

}