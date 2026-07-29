import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { ManagerExpense, ManagerDashboard, ApprovalHistoryItem } from './manager.model';
import { UserProfile } from '../employee/employee.model';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  // Signals for state management
  readonly dashboard = signal<ManagerDashboard | null>(null);
  readonly pendingExpenses = signal<ManagerExpense[]>([]);
  readonly globalHistory = signal<ApprovalHistoryItem[]>([]);
  readonly profile = signal<UserProfile | null>(null);

  // Selected currency symbol (synced with corporate standards)
  readonly selectedCurrencySymbol = signal<string>(localStorage.getItem('selectedCurrencySymbol') || '$');

  constructor(private http: HttpClient) {
    this.initializeData();
  }

  initializeData(): void {
    this.loadDashboard();
    this.loadPendingExpenses();
    this.loadGlobalHistory();
    this.loadProfile();
  }

  loadDashboard(): void {
    this.http.get<ManagerDashboard>('/api/manager/dashboard').subscribe({
      next: (data) => this.dashboard.set(data),
      error: (err) => console.error('Failed to load manager dashboard summary', err)
    });
  }

  loadPendingExpenses(): void {
    this.http.get<ManagerExpense[]>('/api/manager/pending').subscribe({
      next: (data) => this.pendingExpenses.set(data),
      error: (err) => console.error('Failed to load pending expenses list', err)
    });
  }

  loadGlobalHistory(): void {
    this.http.get<ApprovalHistoryItem[]>('/api/manager/history').subscribe({
      next: (data) => this.globalHistory.set(data),
      error: (err) => console.error('Failed to load global approval history', err)
    });
  }

  loadProfile(): void {
    this.http.get<UserProfile>('/api/manager/profile').subscribe({
      next: (data) => this.profile.set(data),
      error: (err) => console.error('Failed to load manager profile details', err)
    });
  }

  updateProfile(updatedProfile: UserProfile): Observable<any> {
    return this.http.put<any>('/api/manager/profile', updatedProfile).pipe(
      tap(() => {
        this.profile.set(updatedProfile);
      })
    );
  }

  getExpenseById(id: string): Observable<ManagerExpense> {
    return this.http.get<ManagerExpense>(`/api/manager/expense/${id}`);
  }

  getExpenseHistory(id: string): Observable<ApprovalHistoryItem[]> {
    return this.http.get<ApprovalHistoryItem[]>(`/api/manager/history/${id}`);
  }

  approveExpense(id: string, notes: string): Observable<any> {
    return this.http.post<any>(`/api/manager/approve/${id}`, { notes }).pipe(
      tap(() => {
        // Refresh local state lists to sync UI
        this.loadDashboard();
        this.loadPendingExpenses();
        this.loadGlobalHistory();
      })
    );
  }

  rejectExpense(id: string, reason: string): Observable<any> {
    return this.http.post<any>(`/api/manager/reject/${id}`, { reason }).pipe(
      tap(() => {
        // Refresh local state lists to sync UI
        this.loadDashboard();
        this.loadPendingExpenses();
        this.loadGlobalHistory();
      })
    );
  }
}
