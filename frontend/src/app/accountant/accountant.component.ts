import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountantService } from './accountant.service';
import { AuthService } from '../auth.service';
import { AccountantExpense } from './accountant.model';

@Component({
  selector: 'app-accountant',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './accountant.component.html',
  styleUrl: './accountant.component.css'
})
export class AccountantComponent implements OnInit {
  // Expose Math to template
  readonly Math = Math;

  // View state: 'dashboard' | 'approved-expenses' | 'expense-details' | 'payment-history'
  activeTab: 'dashboard' | 'approved-expenses' | 'expense-details' | 'payment-history' = 'dashboard';
  showMobileSidebar = false;

  // Search, Filter & Pagination State for Approved list
  searchQuery = '';
  filterCategory = '';
  currentPage = 1;
  pageSize = 5;

  // Search, Filter & Pagination State for Payment History
  historySearchQuery = '';
  historyFilterCategory = '';
  historyCurrentPage = 1;
  historyPageSize = 5;

  // Detailed Expense View state
  selectedExpense: AccountantExpense | null = null;

  // Payment Confirmation Modal state
  showPaymentConfirmModal = false;
  paymentNotes = '';

  // Toast Notification state
  toasts: { id: number; type: 'success' | 'error' | 'info'; message: string }[] = [];
  private toastIdCounter = 0;

  // Categories list
  readonly categories = [
    'Software & SaaS',
    'Meals & Entertainment',
    'Travel',
    'Office Supplies',
    'Others'
  ];

  constructor(
    public accountantService: AccountantService,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Initial fetch
    this.accountantService.initializeData();
  }

  // Active accountant display name
  get accountantName(): string {
    return this.authService.currentUserName() ?? 'Accountant Office';
  }

  // --- Search, Filtering & Pagination for Approved List ---
  get filteredApprovedExpenses(): AccountantExpense[] {
    return this.accountantService.approvedExpenses().filter(e => {
      const matchesSearch = e.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.id.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.employee?.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.description.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchesCategory = !this.filterCategory || e.category === this.filterCategory;
      
      return matchesSearch && matchesCategory;
    });
  }

  get paginatedApprovedExpenses(): AccountantExpense[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredApprovedExpenses.slice(start, start + this.pageSize);
  }

  get totalApprovedPages(): number {
    return Math.ceil(this.filteredApprovedExpenses.length / this.pageSize);
  }

  get totalApprovedCount(): number {
    return this.filteredApprovedExpenses.length;
  }

  changeApprovedPage(page: number): void {
    if (page >= 1 && page <= this.totalApprovedPages) {
      this.currentPage = page;
    }
  }

  getApprovedPagesArray(): number[] {
    return Array.from({ length: this.totalApprovedPages }, (_, i) => i + 1);
  }

  onApprovedFilterChange(): void {
    this.currentPage = 1;
  }

  // --- Search, Filtering & Pagination for Payment History ---
  get filteredPaymentHistory(): AccountantExpense[] {
    return this.accountantService.paymentHistory().filter(e => {
      const matchesSearch = e.title.toLowerCase().includes(this.historySearchQuery.toLowerCase()) ||
                            e.id.toLowerCase().includes(this.historySearchQuery.toLowerCase()) ||
                            e.employee?.name.toLowerCase().includes(this.historySearchQuery.toLowerCase()) ||
                            e.description.toLowerCase().includes(this.historySearchQuery.toLowerCase());
      const matchesCategory = !this.historyFilterCategory || e.category === this.historyFilterCategory;
      
      return matchesSearch && matchesCategory;
    });
  }

  get paginatedPaymentHistory(): AccountantExpense[] {
    const start = (this.historyCurrentPage - 1) * this.historyPageSize;
    return this.filteredPaymentHistory.slice(start, start + this.historyPageSize);
  }

  get totalHistoryPages(): number {
    return Math.ceil(this.filteredPaymentHistory.length / this.historyPageSize);
  }

  get totalHistoryCount(): number {
    return this.filteredPaymentHistory.length;
  }

  changeHistoryPage(page: number): void {
    if (page >= 1 && page <= this.totalHistoryPages) {
      this.historyCurrentPage = page;
    }
  }

  getHistoryPagesArray(): number[] {
    return Array.from({ length: this.totalHistoryPages }, (_, i) => i + 1);
  }

  onHistoryFilterChange(): void {
    this.historyCurrentPage = 1;
  }

  // --- Expense Details Navigation ---
  viewExpenseDetails(expense: AccountantExpense): void {
    this.accountantService.getExpenseById(expense.id).subscribe({
      next: (fullExp) => {
        this.selectedExpense = fullExp;
        this.activeTab = 'expense-details';
        this.showMobileSidebar = false;
      },
      error: (err) => {
        console.error('Failed to load expense details', err);
        this.showToast('error', 'Failed to retrieve expense claim data.');
      }
    });
  }

  // --- Payment Operations ---
  openPaymentModal(): void {
    this.paymentNotes = '';
    this.showPaymentConfirmModal = true;
  }

  closePaymentModal(): void {
    this.showPaymentConfirmModal = false;
    this.paymentNotes = '';
  }

  confirmPayment(): void {
    if (!this.selectedExpense) return;

    const id = this.selectedExpense.id;
    const notes = this.paymentNotes.trim() ? this.paymentNotes.trim() : 'Payment processed via Accountant Portal.';
    this.accountantService.payExpense(id, notes).subscribe({
      next: (res) => {
        this.closePaymentModal();
        this.showToast('success', `Payment for expense claim ${id} has been recorded successfully.`);
        this.activeTab = 'approved-expenses';
        this.selectedExpense = null;
      },
      error: (err) => {
        console.error('Failed to process payment', err);
        this.showToast('error', 'Payment processing failed. Please try again.');
      }
    });
  }

  // --- Toast Manager ---
  showToast(type: 'success' | 'error' | 'info', message: string): void {
    const id = ++this.toastIdCounter;
    this.toasts.push({ id, type, message });
    setTimeout(() => {
      this.toasts = this.toasts.filter(t => t.id !== id);
    }, 4000);
  }

  removeToast(id: number): void {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }

  // --- Sidebar & Navigation ---
  changeTab(tab: 'dashboard' | 'approved-expenses' | 'expense-details' | 'payment-history'): void {
    this.activeTab = tab;
    this.showMobileSidebar = false;
    if (tab === 'dashboard') {
      this.accountantService.loadDashboard();
      this.accountantService.loadApprovedExpenses();
    } else if (tab === 'approved-expenses') {
      this.accountantService.loadApprovedExpenses();
      this.currentPage = 1;
    } else if (tab === 'payment-history') {
      this.accountantService.loadPaymentHistory();
      this.historyCurrentPage = 1;
    }
  }

  logout(): void {
    this.authService.currentUserName.set(null);
    this.authService.currentUserRole.set(null);
    this.router.navigateByUrl('/');
  }
}
