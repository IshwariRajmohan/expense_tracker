import { Component, OnInit, signal, computed, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ManagerService } from './manager.service';
import { AuthService } from '../auth.service';
import { ManagerExpense, ApprovalHistoryItem } from './manager.model';
import { UserProfile } from '../employee/employee.model';

@Component({
  selector: 'app-manager',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manager.component.html',
  styleUrl: './manager.component.css'
})
export class ManagerComponent implements OnInit {
  showNotificationDropdown = false;
  dismissedNotificationIds = signal<string[]>([]);
  // Expose Math to template
  readonly Math = Math;

  // View state: 'dashboard' | 'expense-details' | 'approval-history' | 'profile'
  activeTab: 'dashboard' | 'expense-details' | 'approval-history' | 'profile' = 'dashboard';
  showMobileSidebar = false;

  // Search, Filter & Pagination State for Dashboard Pending list
  searchQuery = '';
  filterCategory = '';
  currentPage = 1;
  pageSize = 5;

  // Detailed Claim View state
  selectedExpense: ManagerExpense | null = null;
  selectedExpenseHistory: ApprovalHistoryItem[] = [];

  // Modals state
  showApproveConfirmModal = false;
  showRejectReasonModal = false;
  rejectReason = '';

  // Toast Notification state
  toasts: { id: number; type: 'success' | 'error' | 'info'; message: string }[] = [];
  private toastIdCounter = 0;

  // Profile Form state
  profileForm: UserProfile = {
    name: 'Ishwari Rajmohan',
    email: 'ishwari.r@firstpay.com',
    role: 'Department Manager',
    department: 'Engineering',
    employeeId: 'FP-2024-001',
    budgetLimit: 50000.00,
    spentAmount: 0.00,
    avatarUrl: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop'
  };

  // Categories list
  readonly categories = [
    'Software & SaaS',
    'Meals & Entertainment',
    'Travel',
    'Office Supplies',
    'Others'
  ];

  constructor(
    public managerService: ManagerService,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Initial fetch
    this.managerService.initializeData();
    // Watch for profile changes to set local profile form
    setTimeout(() => {
      this.initProfileForm();
    }, 1000);
  }

  // Active manager display name
  get managerName(): string {
    return this.authService.currentUserName() ?? 'Ishwari Rajmohan';
  }

  // --- Profile methods ---
  initProfileForm(): void {
    const prof = this.managerService.profile();
    if (prof) {
      this.profileForm = { ...prof };
    }
  }

  updateProfileDetails(): void {
    this.managerService.updateProfile(this.profileForm).subscribe({
      next: (res) => {
        this.showToast('success', 'Profile updated successfully!');
      },
      error: (err) => {
        console.error('Failed to update manager profile', err);
        this.showToast('error', 'Failed to update profile details.');
      }
    });
  }

  // --- Search, Filtering & Pagination for Pending List ---
  get filteredExpenses(): ManagerExpense[] {
    return this.managerService.pendingExpenses().filter(e => {
      const matchesSearch = e.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.id.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.employee?.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.description.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchesCategory = !this.filterCategory || e.category === this.filterCategory;
      
      return matchesSearch && matchesCategory;
    });
  }

  get paginatedExpenses(): ManagerExpense[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredExpenses.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredExpenses.length / this.pageSize);
  }

  get totalExpensesCount(): number {
    return this.filteredExpenses.length;
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  onFilterChange(): void {
    this.currentPage = 1;
  }

  // --- Expense Details Navigation and Data Fetching ---
  viewExpenseDetails(expense: ManagerExpense): void {
    this.managerService.getExpenseById(expense.id).subscribe({
      next: (fullExp) => {
        this.selectedExpense = fullExp;
        this.managerService.getExpenseHistory(expense.id).subscribe({
          next: (hist) => {
            this.selectedExpenseHistory = hist;
            this.activeTab = 'expense-details';
            this.showMobileSidebar = false;
          },
          error: (err) => console.error('Failed to load expense history timeline', err)
        });
      },
      error: (err) => {
        console.error('Failed to load expense details', err);
        this.showToast('error', 'Failed to retrieve expense claim data.');
      }
    });
  }

  // --- Approval Operations ---
  openApproveModal(): void {
    this.showApproveConfirmModal = true;
  }

  closeApproveModal(): void {
    this.showApproveConfirmModal = false;
  }

  confirmApprove(): void {
    if (!this.selectedExpense) return;

    const id = this.selectedExpense.id;
    this.managerService.approveExpense(id, 'Approved via portal auditing.').subscribe({
      next: (res) => {
        this.closeApproveModal();
        this.showToast('success', `Expense claim ${id} approved successfully.`);
        this.activeTab = 'dashboard';
        this.selectedExpense = null;
      },
      error: (err) => {
        console.error('Failed to approve expense claim', err);
        this.showToast('error', 'Approval action failed. Please try again.');
      }
    });
  }

  // --- Rejection Operations ---
  openRejectModal(): void {
    this.rejectReason = '';
    this.showRejectReasonModal = true;
  }

  closeRejectModal(): void {
    this.showRejectReasonModal = false;
    this.rejectReason = '';
  }

  confirmReject(): void {
    if (!this.selectedExpense) return;
    if (!this.rejectReason.trim()) {
      this.showToast('error', 'Rejection reason is mandatory.');
      return;
    }

    const id = this.selectedExpense.id;
    this.managerService.rejectExpense(id, this.rejectReason.trim()).subscribe({
      next: (res) => {
        this.closeRejectModal();
        this.showToast('success', `Expense claim ${id} rejected successfully.`);
        this.activeTab = 'dashboard';
        this.selectedExpense = null;
      },
      error: (err) => {
        console.error('Failed to reject expense claim', err);
        this.showToast('error', 'Rejection action failed. Please try again.');
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
  changeTab(tab: 'dashboard' | 'expense-details' | 'approval-history' | 'profile'): void {
    this.activeTab = tab;
    this.showMobileSidebar = false;
    if (tab === 'dashboard') {
      this.managerService.loadDashboard();
      this.managerService.loadPendingExpenses();
      this.currentPage = 1;
    } else if (tab === 'approval-history') {
      this.managerService.loadGlobalHistory();
    } else if (tab === 'profile') {
      this.managerService.loadProfile();
      this.initProfileForm();
    }
  }

  toggleNotifications(event: MouseEvent): void {
    event.stopPropagation();
    this.showNotificationDropdown = !this.showNotificationDropdown;
  }

  get notificationCount(): number {
    return this.notifications.length;
  }

  get notifications() {
    return this.managerService.pendingExpenses()
      .filter(e => !this.dismissedNotificationIds().includes(e.id))
      .map(e => ({
        id: e.id,
        title: 'Pending Claim Requisition',
        description: `"${e.title}" from ${e.employee?.name || 'Employee'} needs audit.`,
        type: 'pending',
        time: 'Pending',
        expense: e
      }));
  }

  clearNotifications(): void {
    const ids = this.notifications.map(n => n.id);
    this.dismissedNotificationIds.set([...this.dismissedNotificationIds(), ...ids]);
    this.showNotificationDropdown = false;
  }

  handleNotificationClick(note: any): void {
    this.showNotificationDropdown = false;
    this.viewExpenseDetails(note.expense);
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.showNotificationDropdown = false;
  }

  logout(): void {
    this.authService.currentUserName.set(null);
    this.authService.currentUserRole.set(null);
    this.managerService.profile.set(null);
    this.managerService.pendingExpenses.set([]);
    this.managerService.dashboard.set(null);
    this.router.navigateByUrl('/');
  }
}
