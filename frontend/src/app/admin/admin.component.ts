import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from './admin.service';
import { AuthService } from '../auth.service';
import { AdminUser, AdminExpense, ApprovalHistoryItem } from './admin.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {
  // Expose Math to template
  readonly Math = Math;

  // Tabs: 'dashboard' | 'users' | 'expenses' | 'reports' | 'history' | 'freeze-date' | 'settings'
  activeTab: 'dashboard' | 'users' | 'expenses' | 'reports' | 'history' | 'freeze-date' | 'settings' = 'dashboard';
  showMobileSidebar = false;

  // Search & Filters for Users list
  userSearchQuery = '';
  userFilterRole = '';
  userFilterDept = '';
  selectedUser: AdminUser | null = null;

  // Search, Filters & Pagination for Expenses list
  expenseSearchQuery = '';
  expenseFilterStatus = '';
  expenseFilterCategory = '';
  expenseCurrentPage = 1;
  expensePageSize = 5;
  selectedExpense: AdminExpense | null = null;

  // Search & Filters for Approval History
  historySearchQuery = '';
  historyFilterAction = '';
  historyCurrentPage = 1;
  historyPageSize = 8;

  // Add User Form state
  showAddUserModal = false;
  addUserForm: AdminUser = {
    name: '',
    email: '',
    role: 'Employee',
    department: 'Engineering',
    employeeId: '',
    budgetLimit: 5000,
    spentAmount: 0,
    avatarUrl: ''
  };

  // Freeze Date form
  newFreezeDay = 18;

  // Settings form
  settingsForm = {
    companyName: '',
    companyAddress: '',
    corporateCurrency: '',
    systemMode: '',
    adminProfile: {
      name: '',
      email: '',
      role: '',
      department: '',
      employeeId: '',
      avatarUrl: ''
    }
  };

  // Toast Notification state
  toasts: { id: number; type: 'success' | 'error' | 'info'; message: string }[] = [];
  private toastIdCounter = 0;

  // Predefined Categories
  readonly categories = ['Software & SaaS', 'Meals & Entertainment', 'Travel', 'Office Supplies', 'Others'];
  readonly departments = ['Engineering', 'Marketing', 'Sales', 'Finance', 'Operations'];
  readonly roles = ['Employee', 'Manager', 'Accountant'];
  readonly statuses = ['Draft', 'Pending', 'Approved', 'Rejected', 'Paid'];

  constructor(
    public adminService: AdminService,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.adminService.initializeData();
    // Copy settings to local form when available
    setTimeout(() => {
      this.syncSettingsForm();
      const fData = this.adminService.freezeDate();
      if (fData) {
        this.newFreezeDay = fData.freezeDay;
      }
    }, 1000);
  }

  get adminName(): string {
    return this.authService.currentUserName() ?? 'System Admin';
  }

  // --- Sync Settings local state ---
  syncSettingsForm(): void {
    const s = this.adminService.settings();
    if (s) {
      this.settingsForm = JSON.parse(JSON.stringify(s));
    }
  }

  saveSettings(): void {
    this.showToast('success', 'Corporate configurations saved successfully!');
  }

  // --- Add User operations ---
  openAddUserModal(): void {
    this.addUserForm = {
      name: '',
      email: '',
      role: 'Employee',
      department: 'Engineering',
      employeeId: '',
      budgetLimit: 5000,
      spentAmount: 0,
      avatarUrl: ''
    };
    this.showAddUserModal = true;
  }

  closeAddUserModal(): void {
    this.showAddUserModal = false;
  }

  submitAddUser(): void {
    if (!this.addUserForm.name.trim() || !this.addUserForm.email.trim() || !this.addUserForm.employeeId.trim()) {
      this.showToast('error', 'Please fill in Name, Email and Employee ID fields.');
      return;
    }

    this.adminService.addUser(this.addUserForm).subscribe({
      next: (res) => {
        if (res.success) {
          this.showToast('success', res.message);
          this.closeAddUserModal();
        } else {
          this.showToast('error', res.message);
        }
      },
      error: (err) => {
        console.error('Failed to create user', err);
        this.showToast('error', err.error?.message || 'Error occurred while creating user.');
      }
    });
  }

  // --- Filtering for User Grid ---
  get filteredUsers(): AdminUser[] {
    return this.adminService.users().filter(u => {
      const matchesSearch = u.name.toLowerCase().includes(this.userSearchQuery.toLowerCase()) ||
                            u.email.toLowerCase().includes(this.userSearchQuery.toLowerCase()) ||
                            u.employeeId.toLowerCase().includes(this.userSearchQuery.toLowerCase());
      const matchesRole = !this.userFilterRole || u.role.toLowerCase() === this.userFilterRole.toLowerCase();
      const matchesDept = !this.userFilterDept || u.department.toLowerCase() === this.userFilterDept.toLowerCase();

      return matchesSearch && matchesRole && matchesDept;
    });
  }

  // --- Filtering and Pagination for Expenses ---
  get filteredExpenses(): AdminExpense[] {
    return this.adminService.expenses().filter(e => {
      const matchesSearch = e.title.toLowerCase().includes(this.expenseSearchQuery.toLowerCase()) ||
                            e.id.toLowerCase().includes(this.expenseSearchQuery.toLowerCase()) ||
                            e.employee?.name.toLowerCase().includes(this.expenseSearchQuery.toLowerCase()) ||
                            e.description.toLowerCase().includes(this.expenseSearchQuery.toLowerCase());
      const matchesStatus = !this.expenseFilterStatus || e.status.toLowerCase() === this.expenseFilterStatus.toLowerCase();
      const matchesCategory = !this.expenseFilterCategory || e.category.toLowerCase() === this.expenseFilterCategory.toLowerCase();

      return matchesSearch && matchesStatus && matchesCategory;
    });
  }

  get paginatedExpenses(): AdminExpense[] {
    const start = (this.expenseCurrentPage - 1) * this.expensePageSize;
    return this.filteredExpenses.slice(start, start + this.expensePageSize);
  }

  get totalExpensePages(): number {
    return Math.ceil(this.filteredExpenses.length / this.expensePageSize);
  }

  changeExpensePage(page: number): void {
    if (page >= 1 && page <= this.totalExpensePages) {
      this.expenseCurrentPage = page;
    }
  }

  getExpensePagesArray(): number[] {
    return Array.from({ length: this.totalExpensePages }, (_, i) => i + 1);
  }

  onExpenseFilterChange(): void {
    this.expenseCurrentPage = 1;
  }

  viewExpenseDetails(expense: AdminExpense): void {
    this.adminService.getExpenseById(expense.id).subscribe({
      next: (fullExp) => {
        this.selectedExpense = fullExp;
        this.activeTab = 'expenses';
      },
      error: (err) => {
        console.error('Failed to load expense details', err);
        this.showToast('error', 'Failed to retrieve expense claim data.');
      }
    });
  }

  // --- Filtering and Pagination for Approval History timeline ---
  get filteredHistory(): ApprovalHistoryItem[] {
    return this.adminService.workflowHistory().filter(h => {
      const matchesSearch = h.expenseId.toLowerCase().includes(this.historySearchQuery.toLowerCase()) ||
                            h.performedBy.toLowerCase().includes(this.historySearchQuery.toLowerCase()) ||
                            h.notes.toLowerCase().includes(this.historySearchQuery.toLowerCase());
      const matchesAction = !this.historyFilterAction || h.action.toLowerCase() === this.historyFilterAction.toLowerCase();

      return matchesSearch && matchesAction;
    });
  }

  get paginatedHistory(): ApprovalHistoryItem[] {
    const start = (this.historyCurrentPage - 1) * this.historyPageSize;
    return this.filteredHistory.slice(start, start + this.historyPageSize);
  }

  get totalHistoryPages(): number {
    return Math.ceil(this.filteredHistory.length / this.historyPageSize);
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

  // --- Freeze Date operation ---
  submitFreezeDateUpdate(): void {
    if (this.newFreezeDay < 1 || this.newFreezeDay > 31) {
      this.showToast('error', 'Day must be between 1 and 31.');
      return;
    }

    this.adminService.updateFreezeDate(this.newFreezeDay).subscribe({
      next: (res) => {
        if (res.success) {
          this.showToast('success', res.message);
        } else {
          this.showToast('error', res.message);
        }
      },
      error: (err) => {
        console.error('Failed to update freeze date', err);
        this.showToast('error', 'Error calling api to update freeze date.');
      }
    });
  }

  // --- Export to CSV / PDF Mock Trigger ---
  exportExpensesToCsv(): void {
    const list = this.filteredExpenses;
    if (list.length === 0) {
      this.showToast('error', 'No expenses available to export.');
      return;
    }

    const headers = ['Expense ID', 'Title', 'Category', 'Claim Date', 'Filing Employee', 'Department', 'Total Amount', 'Status', 'Paid Date'];
    const csvRows = [headers.join(',')];

    for (const e of list) {
      const values = [
        e.id,
        `"${e.title.replace(/"/g, '""')}"`,
        e.category,
        e.date,
        e.employee?.name || '',
        e.employee?.department || '',
        e.totalAmount,
        e.status,
        e.paymentDate || ''
      ];
      csvRows.push(values.join(','));
    }

    this.triggerFileDownload(csvRows.join('\n'), 'Global_Expense_Report.csv', 'text/csv');
    this.showToast('success', 'Excel-compatible CSV generated successfully.');
  }

  exportExpensesToPdf(): void {
    // Premium PDF printing mock - opens the system print dialog for a clean print template
    this.showToast('info', 'Preparing print audit document...');
    setTimeout(() => {
      window.print();
    }, 500);
  }

  downloadReportCsv(type: string): void {
    const r = this.adminService.reports();
    if (!r) {
      this.showToast('error', 'Report data not loaded.');
      return;
    }

    let csvContent = '';
    let filename = '';

    if (type === 'department') {
      csvContent = 'Department,Total Claims,Sum Reimbursed\n' +
        r.departmentWiseExpenses.map(d => `"${d.departmentName}",${d.count},${d.totalAmount}`).join('\n');
      filename = 'Department_Wise_Expense_Summary.csv';
    } else if (type === 'employee') {
      csvContent = 'Employee ID,Employee Name,Department,Claims Count,Total Reimbursed\n' +
        r.employeeWiseExpenses.map(e => `"${e.employeeId}","${e.employeeName}","${e.department}",${e.count},${e.totalAmount}`).join('\n');
      filename = 'Employee_Wise_Expense_Summary.csv';
    } else if (type === 'top-spenders') {
      csvContent = 'Employee ID,Employee Name,Department,Claims Count,Total Reimbursed\n' +
        r.topSpendingEmployees.map(e => `"${e.employeeId}","${e.employeeName}","${e.department}",${e.count},${e.totalAmount}`).join('\n');
      filename = 'Top_Spending_Employees_Report.csv';
    } else {
      csvContent = 'Month,Reimbursed Amount\n' +
        r.monthlyExpenseReport.map(m => `"${m.label}",${m.value}`).join('\n');
      filename = 'Monthly_Expense_Report.csv';
    }

    this.triggerFileDownload(csvContent, filename, 'text/csv');
    this.showToast('success', `Report "${filename}" downloaded successfully.`);
  }

  private triggerFileDownload(content: string, filename: string, mimeType: string): void {
    const blob = new Blob([content], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
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
  changeTab(tab: 'dashboard' | 'users' | 'expenses' | 'reports' | 'history' | 'freeze-date' | 'settings'): void {
    this.activeTab = tab;
    this.showMobileSidebar = false;
    this.selectedUser = null;
    this.selectedExpense = null;

    if (tab === 'dashboard') {
      this.adminService.loadDashboard();
    } else if (tab === 'users') {
      this.adminService.loadUsers();
    } else if (tab === 'expenses') {
      this.adminService.loadExpenses();
      this.expenseCurrentPage = 1;
    } else if (tab === 'reports') {
      this.adminService.loadReports();
    } else if (tab === 'history') {
      this.adminService.loadWorkflowHistory();
      this.historyCurrentPage = 1;
    } else if (tab === 'freeze-date') {
      this.adminService.loadFreezeDate();
      const f = this.adminService.freezeDate();
      if (f) {
        this.newFreezeDay = f.freezeDay;
      }
    } else if (tab === 'settings') {
      this.adminService.loadSettings();
      this.syncSettingsForm();
    }
  }

  logout(): void {
    this.authService.currentUserName.set(null);
    this.authService.currentUserRole.set(null);
    this.router.navigateByUrl('/');
  }
}
