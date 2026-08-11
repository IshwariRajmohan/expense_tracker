import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Expense, UserProfile, ActivityLog, ExpenseItem } from './employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  // Signals for state management
  readonly expenses = signal<Expense[]>([]);
  readonly profile = signal<UserProfile | null>(null);
  readonly activities = signal<ActivityLog[]>([]);
  readonly isSubmissionFrozen = signal<boolean>(false);
  readonly freezeDay = signal<number>(18);

  // Currency and country state
  readonly selectedCountry = signal<string>(localStorage.getItem('selectedCountry') || 'United States');
  readonly selectedCurrencySymbol = signal<string>(localStorage.getItem('selectedCurrencySymbol') || '$');

  readonly countries = [
    { name: 'United States', code: 'USD', symbol: '$' },
    { name: 'USA', code: 'USD', symbol: '$' },
    { name: 'United Kingdom', code: 'GBP', symbol: '£' },
    { name: 'UK', code: 'GBP', symbol: '£' },
    { name: 'India', code: 'INR', symbol: '₹' },
    { name: 'Germany', code: 'EUR', symbol: '€' },
    { name: 'France', code: 'EUR', symbol: '€' },
    { name: 'Italy', code: 'EUR', symbol: '€' },
    { name: 'Spain', code: 'EUR', symbol: '€' },
    { name: 'Netherlands', code: 'EUR', symbol: '€' },
    { name: 'Europe', code: 'EUR', symbol: '€' },
    { name: 'Japan', code: 'JPY', symbol: '¥' },
    { name: 'Canada', code: 'CAD', symbol: 'C$' },
    { name: 'Australia', code: 'AUD', symbol: 'A$' },
    { name: 'Switzerland', code: 'CHF', symbol: 'CHF' },
    { name: 'China', code: 'CNY', symbol: '¥' },
    { name: 'Singapore', code: 'SGD', symbol: 'S$' },
    { name: 'Brazil', code: 'BRL', symbol: 'R$' },
    { name: 'Russia', code: 'RUB', symbol: '₽' },
    { name: 'South Africa', code: 'ZAR', symbol: 'R' },
    { name: 'Mexico', code: 'MXN', symbol: 'Mex$' },
    { name: 'United Arab Emirates', code: 'AED', symbol: 'AED' },
    { name: 'Saudi Arabia', code: 'SAR', symbol: 'SAR' },
    { name: 'Turkey', code: 'TRY', symbol: '₺' },
    { name: 'South Korea', code: 'KRW', symbol: '₩' },
    { name: 'New Zealand', code: 'NZD', symbol: 'NZ$' },
    { name: 'Sweden', code: 'SEK', symbol: 'kr' },
    { name: 'Norway', code: 'NOK', symbol: 'kr' },
    { name: 'Denmark', code: 'DKK', symbol: 'kr' },
    { name: 'Poland', code: 'PLN', symbol: 'zł' },
    { name: 'Hong Kong', code: 'HKD', symbol: 'HK$' },
    { name: 'Malaysia', code: 'MYR', symbol: 'RM' },
    { name: 'Thailand', code: 'THB', symbol: '฿' },
    { name: 'Indonesia', code: 'IDR', symbol: 'Rp' },
    { name: 'Philippines', code: 'PHP', symbol: '₱' },
    { name: 'Vietnam', code: 'VND', symbol: '₫' },
    { name: 'Pakistan', code: 'PKR', symbol: '₨' },
    { name: 'Bangladesh', code: 'BDT', symbol: '৳' },
    { name: 'Egypt', code: 'EGP', symbol: 'E£' },
    { name: 'Nigeria', code: 'NGN', symbol: '₦' },
    { name: 'Kenya', code: 'KES', symbol: 'KSh' },
    { name: 'Israel', code: 'ILS', symbol: '₪' }
  ];

  updateCountryCurrency(countryName: string): void {
    this.selectedCountry.set(countryName);
    localStorage.setItem('selectedCountry', countryName);

    const found = this.countries.find(c => c.name.toLowerCase() === countryName.trim().toLowerCase());
    if (found) {
      this.selectedCurrencySymbol.set(found.symbol);
      localStorage.setItem('selectedCurrencySymbol', found.symbol);
    }
  }

  // Computed state
  readonly totalSpent = computed(() => {
    return this.expenses()
      .filter(e => e.status === 'Approved' || e.status === 'Paid')
      .reduce((sum, e) => sum + e.totalAmount, 0);
  });

  readonly pendingAmount = computed(() => {
    return this.expenses()
      .filter(e => e.status === 'Pending')
      .reduce((sum, e) => sum + e.totalAmount, 0);
  });

  readonly remainingBudget = computed(() => {
    const prof = this.profile();
    if (!prof) return 0;
    return Math.max(0, prof.budgetLimit - this.totalSpent());
  });

  constructor(private http: HttpClient) {
    this.initializeData();
  }

  initializeData(): void {
    this.loadProfile();
    this.loadExpenses();
    this.loadActivities();
  }

  loadProfile(): void {
    this.http.get<UserProfile>('/api/employee/profile').subscribe({
      next: (prof) => this.profile.set(prof),
      error: (err) => console.error('Failed to load user profile', err)
    });
  }

  loadExpenses(): void {
    this.http.get<Expense[]>('/api/employee/expenses').subscribe({
      next: (exp) => this.expenses.set(exp),
      error: (err) => console.error('Failed to load expenses list', err)
    });
  }

  loadActivities(): void {
    this.http.get<any>('/api/employee/dashboard').subscribe({
      next: (summary) => {
        if (summary) {
          if (summary.recentActivities) {
            this.activities.set(summary.recentActivities);
          }
          this.isSubmissionFrozen.set(summary.isSubmissionFrozen ?? false);
          this.freezeDay.set(summary.freezeDay ?? 18);
        }
      },
      error: (err) => console.error('Failed to load dashboard activities', err)
    });
  }

  addExpense(expenseData: Omit<Expense, 'id' | 'status'> & { status?: 'Pending' | 'Approved' | 'Rejected' | 'Draft' }): void {
    const isDraft = expenseData.status === 'Draft';
    const endpoint = isDraft ? '/api/employee/save-draft' : '/api/employee/submit';
    
    this.http.post<Expense>(endpoint, expenseData).subscribe({
      next: (newExpense) => {
        this.expenses.set([newExpense, ...this.expenses()]);
        this.loadActivities();
        this.loadProfile(); // reload profile budget totals
      },
      error: (err) => console.error('Failed to submit/save expense', err)
    });
  }

  updateExpense(updatedExpense: Expense): void {
    this.http.put<any>(`/api/employee/expenses/${updatedExpense.id}`, updatedExpense).subscribe({
      next: () => {
        this.loadExpenses();
        this.loadActivities();
        this.loadProfile();
      },
      error: (err) => console.error('Failed to update expense', err)
    });
  }

  deleteExpense(id: string): boolean {
    this.http.delete<any>(`/api/employee/expenses/${id}`).subscribe({
      next: () => {
        this.expenses.set(this.expenses().filter(e => e.id !== id));
        this.loadActivities();
        this.loadProfile();
      },
      error: (err) => console.error('Failed to delete draft expense', err)
    });
    return true;
  }

  updateProfile(updatedProfile: UserProfile): void {
    this.http.put<any>('/api/employee/profile', updatedProfile).subscribe({
      next: () => {
        this.profile.set(updatedProfile);
        this.loadActivities();
      },
      error: (err) => console.error('Failed to update profile details', err)
    });
  }

  changePassword(oldPassword: string, newPassword: string): Observable<any> {
    return this.http.put<any>('/api/employee/change-password', { oldPassword, newPassword });
  }
}
