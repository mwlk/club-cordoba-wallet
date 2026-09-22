import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

// Centraliza la comunicación HTTP: base URL, y punto único donde
// engancharían headers comunes a futuro. Los feature services no
// conocen la URL base ni usan HttpClient directo.
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(path: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${path}`, { params });
  }

  post<T>(path: string, body: object): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${path}`, body);
  }
}
