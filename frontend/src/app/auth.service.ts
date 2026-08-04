import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, of, tap } from 'rxjs';

export interface LoginResponse {
  success: boolean;
  message: string;
  name?: string;
  role?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  readonly currentUserName = signal<string | null>(null);
  readonly currentUserRole = signal<string | null>(null);
  private readonly apiUrl = '/api/auth/login';

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl, { username, password }).pipe(
      tap((response) => {
        if (response.success) {
          this.currentUserName.set(response.name ?? 'Himeshwar');
          this.currentUserRole.set(response.role ?? 'User');
        }
      }),
      catchError((error: HttpErrorResponse) => {
        return of({
          success: false,
          message: error.error?.message || 'Invalid username or password.'
        });
      })
    );
  }
}
