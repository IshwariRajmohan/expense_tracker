export interface ExpenseItem {
  id?: string;
  name: string;
  category: string;
  cost: number;
  quantity: number;
}

export interface Expense {
  id: string;
  title: string;
  category: string;
  date: string;
  description: string;
  items: ExpenseItem[];
  totalAmount: number;
  status: 'Pending' | 'Approved' | 'Rejected' | 'Paid' | 'Draft';
  notes?: string;
}

export interface UserProfile {
  name: string;
  email: string;
  role: string;
  department: string;
  employeeId: string;
  budgetLimit: number;
  spentAmount: number;
  avatarUrl: string;
}

export interface ActivityLog {
  id: string;
  action: string;
  timestamp: string; // ISO string representation
  statusType: 'success' | 'warning' | 'info' | 'danger';
}
