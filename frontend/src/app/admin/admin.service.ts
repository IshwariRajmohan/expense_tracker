import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { 
  AdminDashboard, 
  AdminUser, 
  AdminExpense, 
  AdminReports, 
  AdminFreezeDate, 
  AdminSettings,
  ApprovalHistoryItem
} from './admin.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  readonly dashboard = signal<AdminDashboard | null>(null);
  readonly users = signal<AdminUser[]>([]);
  readonly expenses = signal<AdminExpense[]>([]);
  readonly reports = signal<AdminReports | null>(null);
  readonly workflowHistory = signal<ApprovalHistoryItem[]>([]);
  readonly freezeDate = signal<AdminFreezeDate | null>(null);
  readonly settings = signal<AdminSettings | null>(null);

  readonly selectedCurrencySymbol = signal<string>(localStorage.getItem('selectedCurrencySymbol') || '$');

  constructor(private http: HttpClient) {
    this.initializeData();
  }

  initializeData(): void {
    this.loadDashboard();
    this.loadUsers();
    this.loadExpenses();
    this.loadReports();
    this.loadWorkflowHistory();
    this.loadFreezeDate();
    this.loadSettings();
  }

  loadDashboard(): void {
    this.http.get<AdminDashboard>('/api/admin/dashboard').subscribe({
      next: (data) => this.dashboard.set(data),
      error: (err) => console.error('Failed to load admin dashboard', err)
    });
  }

  loadUsers(): void {
    this.http.get<AdminUser[]>('/api/admin/users').subscribe({
      next: (data) => this.users.set(data),
      error: (err) => console.error('Failed to load admin users', err)
    });
  }

  loadExpenses(): void {
    this.http.get<AdminExpense[]>('/api/admin/expenses').subscribe({
      next: (data) => this.expenses.set(data),
      error: (err) => console.error('Failed to load admin expenses', err)
    });
  }

  loadReports(): void {
    this.http.get<AdminReports>('/api/admin/reports').subscribe({
      next: (data) => this.reports.set(data),
      error: (err) => console.error('Failed to load admin reports', err)
    });
  }

  loadWorkflowHistory(): void {
    this.http.get<ApprovalHistoryItem[]>('/api/admin/history').subscribe({
      next: (data) => this.workflowHistory.set(data),
      error: (err) => console.error('Failed to load admin global history', err)
    });
  }

  loadFreezeDate(): void {
    this.http.get<AdminFreezeDate>('/api/admin/freeze-date').subscribe({
      next: (data) => this.freezeDate.set(data),
      error: (err) => console.error('Failed to load freeze date info', err)
    });
  }

  loadSettings(): void {
    this.http.get<AdminSettings>('/api/admin/settings').subscribe({
      next: (data) => this.settings.set(data),
      error: (err) => console.error('Failed to load admin settings', err)
    });
  }

  getExpenseById(id: string): Observable<AdminExpense> {
    return this.http.get<AdminExpense>(`/api/admin/expense/${id}`);
  }

  updateFreezeDate(day: number): Observable<any> {
    return this.http.put<any>('/api/admin/freeze-date', { day }).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.freezeDate.set(res.data);
          this.loadDashboard();
        }
      })
    );
  }

  addUser(user: AdminUser): Observable<any> {
    return this.http.post<any>('/api/admin/users', user).pipe(
      tap((res) => {
        if (res.success) {
          this.loadUsers();
          this.loadDashboard();
        }
      })
    );
  }
}
