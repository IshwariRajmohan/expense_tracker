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

  validateCredentials(username: string, password: string): { success: boolean; name: string; role: string } | null {
    const u = username.trim().toLowerCase();
    if (u === 'himesh' && password === '123') {
      return { success: true, name: 'Himeshwar', role: 'Employee' };
    }
    if (u === 'manager' && password === '123') {
      return { success: true, name: 'Ishwari Rajmohan', role: 'Manager' };
    }
    return null;
  }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl, { username, password }).pipe(
      tap((response) => {
        if (response.success) {
          this.currentUserName.set(response.name ?? 'Himeshwar');
          this.currentUserRole.set(response.role ?? 'User');
        }
      }),
      catchError((error: HttpErrorResponse) => {
        const creds = this.validateCredentials(username, password);
        const fallback = creds
          ? {
              success: true,
              message: 'Login successful',
              name: creds.name,
              role: creds.role
            }
          : {
              success: false,
              message: 'Invalid username or password.'
            };

        if (fallback.success) {
          this.currentUserName.set(fallback.name ?? 'Himeshwar');
          this.currentUserRole.set(fallback.role ?? 'Employee');
        }

        return of(fallback);
      })
    );
  }
}
