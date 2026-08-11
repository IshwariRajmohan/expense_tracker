import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AccountantDashboard, AccountantExpense, ActivityLog } from './accountant.model';

@Injectable({
  providedIn: 'root'
})
export class AccountantService {
  readonly dashboard = signal<AccountantDashboard | null>(null);
  readonly approvedExpenses = signal<AccountantExpense[]>([]);
  readonly paymentHistory = signal<AccountantExpense[]>([]);
  readonly activityLogs = signal<ActivityLog[]>([]);
  
  readonly selectedCurrencySymbol = signal<string>(localStorage.getItem('selectedCurrencySymbol') || '$');

  constructor(private http: HttpClient) {
    this.initializeData();
  }

  initializeData(): void {
    this.loadDashboard();
    this.loadApprovedExpenses();
    this.loadPaymentHistory();
    this.loadActivityLogs();
  }

  loadDashboard(): void {
    this.http.get<AccountantDashboard>('/api/accountant/dashboard').subscribe({
      next: (data) => this.dashboard.set(data),
      error: (err) => console.error('Failed to load accountant dashboard', err)
    });
  }

  loadApprovedExpenses(): void {
    this.http.get<AccountantExpense[]>('/api/accountant/approved').subscribe({
      next: (data) => this.approvedExpenses.set(data),
      error: (err) => console.error('Failed to load approved expenses', err)
    });
  }

  loadPaymentHistory(): void {
    this.http.get<AccountantExpense[]>('/api/accountant/payment-history').subscribe({
      next: (data) => this.paymentHistory.set(data),
      error: (err) => console.error('Failed to load payment history', err)
    });
  }

  loadActivityLogs(): void {
    this.http.get<ActivityLog[]>('/api/accountant/activity-logs').subscribe({
      next: (data) => this.activityLogs.set(data),
      error: (err) => console.error('Failed to load activity logs', err)
    });
  }

  getExpenseById(id: string): Observable<AccountantExpense> {
    return this.http.get<AccountantExpense>(`/api/accountant/expense/${id}`);
  }

  payExpense(id: string, notes?: string): Observable<any> {
    return this.http.post<any>(`/api/accountant/pay/${id}`, { notes }).pipe(
      tap(() => {
        this.loadDashboard();
        this.loadApprovedExpenses();
        this.loadPaymentHistory();
        this.loadActivityLogs();
      })
    );
  }
}
