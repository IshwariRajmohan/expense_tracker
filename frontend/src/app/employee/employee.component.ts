import { Component, OnInit, signal, computed, effect, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { EmployeeService } from './employee.service';
import { AuthService } from '../auth.service';
import { Expense, ExpenseItem, UserProfile } from './employee.model';

@Component({
  selector: 'app-employee',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee.component.html',
  styleUrl: './employee.component.css'
})
export class EmployeeComponent implements OnInit {
  showNotificationDropdown = false;
  dismissedNotificationIds = signal<string[]>([]);
  // Expose Math to template
  readonly Math = Math;

  // Country Selection Input Text
  countryInputText = '';

  // Navigation State
  activeTab: 'dashboard' | 'create-expense' | 'my-expenses' | 'profile' = 'dashboard';
  showMobileSidebar = false;

  // Form State for Expense Submission / Modification
  isEditing = false;
  editingExpenseId = '';
  expenseTitle = '';
  expenseCategory = 'Software & SaaS';
  expenseDate = '';
  expenseDescription = '';
  expenseNotes = '';
  expenseItems: ExpenseItem[] = [];

  // Modal State
  selectedExpense: Expense | null = null;
  showDetailsModal = false;
  showDeleteConfirmModal = false;
  deleteConfirmExpenseId = '';

  // Notification State
  showRejectedNotification = false;
  rejectedCount = 0;

  // Filter & Pagination State
  searchQuery = '';
  filterCategory = '';
  filterStatus = '';
  currentPage = 1;
  pageSize = 5;

  // Local Profile Edit Form Model
  profileForm: UserProfile = {
    name: '',
    email: '',
    role: '',
    department: '',
    employeeId: '',
    budgetLimit: 0,
    spentAmount: 0,
    avatarUrl: ''
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
    public employeeService: EmployeeService,
    public authService: AuthService,
    private router: Router
  ) {
    // Check for rejected expenses to show notification
    effect(() => {
      const rejected = this.employeeService.expenses().filter(e => e.status?.toLowerCase() === 'rejected');
      if (rejected.length > 0) {
        this.rejectedCount = rejected.length;
        this.showRejectedNotification = true;
      } else {
        this.showRejectedNotification = false;
      }
    });

    // Reactively sync the profile form when profile details load
    effect(() => {
      const prof = this.employeeService.profile();
      if (prof) {
        this.profileForm = { ...prof };
      }
    });
  }

  ngOnInit(): void {
    this.employeeService.initializeData();
    this.resetExpenseForm();
    this.initProfileForm();
    this.countryInputText = this.employeeService.selectedCountry();
  }

  onCountryInputChange(value: string): void {
    this.countryInputText = value;
    this.employeeService.updateCountryCurrency(value);
  }

  // Active user name from AuthService
  get username(): string {
    return this.authService.currentUserName() ?? 'Himeshwar';
  }

  // --- Profile Actions ---
  initProfileForm(): void {
    const prof = this.employeeService.profile();
    if (prof) {
      this.profileForm = { ...prof };
    }
  }

  updateProfileDetails(): void {
    this.employeeService.updateProfile(this.profileForm);
    // Show success activity and stay on profile
    alert('Profile updated successfully!');
  }

  // --- Dynamic Expense Form Actions ---
  resetExpenseForm(): void {
    this.isEditing = false;
    this.editingExpenseId = '';
    this.expenseTitle = '';
    this.expenseCategory = 'Software & SaaS';
    this.expenseDate = new Date().toISOString().split('T')[0];
    this.expenseDescription = '';
    this.expenseNotes = '';
    this.expenseItems = [
      { name: '', category: 'Software & SaaS', cost: 0, quantity: 1 }
    ];
  }

  addNewItemRow(): void {
    this.expenseItems.push({
      name: '',
      category: this.expenseCategory,
      cost: 0,
      quantity: 1
    });
  }

  removeItemRow(index: number): void {
    if (this.expenseItems.length > 1) {
      this.expenseItems.splice(index, 1);
    } else {
      alert('An expense must contain at least one line item.');
    }
  }

  get formTotalAmount(): number {
    return this.expenseItems.reduce((sum, item) => sum + (item.cost * item.quantity), 0);
  }



  submitExpenseForm(): void {
    if (!this.expenseTitle.trim()) {
      alert('Please enter an expense title.');
      return;
    }
    
    // Validate items
    const invalidItem = this.expenseItems.find(item => !item.name.trim() || item.cost <= 0 || item.quantity <= 0);
    if (invalidItem) {
      alert('Please fill out all item names with valid positive costs and quantities.');
      return;
    }

    // Validate allowance limit
    const remainingAllowance = this.employeeService.remainingBudget();
    if (this.formTotalAmount > remainingAllowance) {
      alert(`Cannot submit expense. The total amount (${this.formTotalAmount.toFixed(2)}) exceeds your available allowance (${remainingAllowance.toFixed(2)}).`);
      return;
    }

    const expensePayload = {
      title: this.expenseTitle,
      category: this.expenseCategory,
      date: this.expenseDate,
      description: this.expenseDescription,
      notes: this.expenseNotes,
      items: this.expenseItems,
      totalAmount: this.formTotalAmount
    };

    if (this.isEditing) {
      const existing = this.employeeService.expenses().find(e => e.id === this.editingExpenseId);
      if (existing) {
        let newStatus = existing.status;
        const lowerStatus = existing.status?.toLowerCase();
        if (lowerStatus === 'rejected' || lowerStatus === 'draft' || lowerStatus === 'pending') {
          newStatus = 'Pending';
        }

        const updatedExpense: Expense = {
          ...existing,
          ...expensePayload,
          status: newStatus
        };
        this.employeeService.updateExpense(updatedExpense);
      }
    } else {
      this.employeeService.addExpense({
        ...expensePayload,
        status: 'Pending'
      });
    }

    this.resetExpenseForm();
    this.activeTab = 'my-expenses';
  }

  get isEditingRejected(): boolean {
    if (!this.isEditing || !this.editingExpenseId) return false;
    const exp = this.employeeService.expenses().find(e => e.id === this.editingExpenseId);
    return exp?.status?.toLowerCase() === 'rejected';
  }

  // --- CRUD Operations ---
  openEditForm(expense: Expense): void {
    this.isEditing = true;
    this.editingExpenseId = expense.id;
    this.expenseTitle = expense.title;
    this.expenseCategory = expense.category;
    this.expenseDate = expense.date;
    this.expenseDescription = expense.description;
    this.expenseNotes = expense.notes ?? '';
    this.expenseItems = expense.items.map(item => ({ ...item }));
    this.activeTab = 'create-expense';
  }

  openDeleteConfirm(id: string): void {
    this.deleteConfirmExpenseId = id;
    this.showDeleteConfirmModal = true;
  }

  closeDeleteConfirm(): void {
    this.deleteConfirmExpenseId = '';
    this.showDeleteConfirmModal = false;
  }

  confirmDelete(): void {
    if (this.deleteConfirmExpenseId) {
      this.employeeService.deleteExpense(this.deleteConfirmExpenseId);
      this.closeDeleteConfirm();
      // Adjust page number if last item deleted
      const totalFiltered = this.filteredExpenses.length;
      const maxPage = Math.max(1, Math.ceil(totalFiltered / this.pageSize));
      if (this.currentPage > maxPage) {
        this.currentPage = maxPage;
      }
    }
  }

  // --- Details Modal ---
  openDetails(expense: Expense): void {
    this.selectedExpense = expense;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.selectedExpense = null;
    this.showDetailsModal = false;
  }

  editRejectedClaim(expense: Expense): void {
    this.showDetailsModal = false;
    this.selectedExpense = null;
    this.openEditForm(expense);
  }

  // --- Search, Filtering & Pagination logic ---
  get filteredExpenses(): Expense[] {
    return this.employeeService.expenses().filter(e => {
      const matchesSearch = e.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.id.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            e.description.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchesCategory = !this.filterCategory || e.category === this.filterCategory;
      const matchesStatus = !this.filterStatus || e.status === this.filterStatus;
      
      return matchesSearch && matchesCategory && matchesStatus;
    });
  }

  get paginatedExpenses(): Expense[] {
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

  // Reset page when filters change
  onFilterChange(): void {
    this.currentPage = 1;
  }

  // --- Category Chart segments ---
  get categoryChartSegments() {
    const expenses = this.employeeService.expenses();
    const total = expenses.reduce((sum, e) => sum + e.totalAmount, 0);

    const categoriesList = [
      { name: 'Software & SaaS', color: '#6366f1' },
      { name: 'Meals & Entertainment', color: '#f43f5e' },
      { name: 'Travel', color: '#10b981' },
      { name: 'Office Supplies', color: '#f59e0b' },
      { name: 'Others', color: '#06b6d4' }
    ];

    let accumulatedPercentage = 0;
    return categoriesList.map(cat => {
      const catExpenses = expenses.filter(e => e.category === cat.name);
      const amount = catExpenses.reduce((sum, e) => sum + e.totalAmount, 0);
      const percentage = total > 0 ? (amount / total) * 100 : 0;

      // Circular offset calculation: circumference = 100
      const offset = 100 - accumulatedPercentage + 25; // add 25 to start at 12 o'clock
      accumulatedPercentage += percentage;

      return {
        ...cat,
        amount,
        percentage,
        strokeDashArray: `${percentage.toFixed(1)} ${(100 - percentage).toFixed(1)}`,
        strokeDashOffset: offset.toFixed(1)
      };
    });
  }

  // --- Monthly Bar Chart data ---
  get monthlyBarChartData() {
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const result = [];
    
    // Find the latest expense date to anchor the 6-month window, fallback to current date
    let anchorDate = new Date();
    const expenses = this.employeeService.expenses();
    if (expenses.length > 0) {
      const dates = expenses.map(e => new Date(e.date)).filter(d => !isNaN(d.getTime()));
      if (dates.length > 0) {
        anchorDate = new Date(Math.max(...dates.map(d => d.getTime())));
      }
    }
    
    const anchorYear = anchorDate.getFullYear();
    const anchorMonth = anchorDate.getMonth(); // 0-11

    for (let i = 5; i >= 0; i--) {
      // Calculate year and month for the i-th month before the anchor
      let targetMonth = anchorMonth - i;
      let targetYear = anchorYear;
      while (targetMonth < 0) {
        targetMonth += 12;
        targetYear -= 1;
      }
      
      const monthName = months[targetMonth];

      const total = expenses
        .filter(e => {
          // Parse YYYY-MM-DD string safely
          const parts = e.date.split('-');
          if (parts.length < 2) return false;
          const expYear = parseInt(parts[0], 10);
          const expMonth = parseInt(parts[1], 10) - 1; // 0-indexed
          
          return expMonth === targetMonth &&
                 expYear === targetYear;
        })
        .reduce((sum, e) => sum + e.totalAmount, 0);

      result.push({
        label: monthName,
        value: total
      });
    }

    const maxVal = Math.max(...result.map(r => r.value), 500);
    return result.map(r => ({
      ...r,
      heightPercentage: (r.value / maxVal) * 100
    }));
  }

  // --- Sidebar & Navigation Actions ---
  changeTab(tab: 'dashboard' | 'create-expense' | 'my-expenses' | 'profile'): void {
    this.activeTab = tab;
    this.showMobileSidebar = false;
    if (tab === 'profile') {
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
    return this.employeeService.expenses()
      .filter(e => e.status?.toLowerCase() === 'rejected' && !this.dismissedNotificationIds().includes(e.id))
      .map(e => ({
        id: e.id,
        title: 'Expense Claim Rejected',
        description: `"${e.title}" was rejected. Tap to edit/resubmit.`,
        type: 'rejected',
        time: 'Just Now',
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
    this.openEditForm(note.expense);
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.showNotificationDropdown = false;
  }

  logout(): void {
    this.authService.currentUserName.set(null);
    this.authService.currentUserRole.set(null);
    this.employeeService.profile.set(null);
    this.employeeService.expenses.set([]);
    this.router.navigateByUrl('/');
  }
}
