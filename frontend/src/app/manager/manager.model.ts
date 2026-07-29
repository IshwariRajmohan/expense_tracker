import { ExpenseItem } from '../employee/employee.model';

export interface EmployeeInfo {
  name: string;
  email: string;
  employeeId: string;
  department: string;
  avatarUrl: string;
}

export interface ManagerExpense {
  id: string;
  title: string;
  category: string;
  date: string;
  description: string;
  items: ExpenseItem[];
  totalAmount: number;
  status: 'Pending' | 'Approved' | 'Rejected' | 'Paid' | 'Draft';
  notes?: string;
  employee?: EmployeeInfo;
}

export interface ApprovalHistoryItem {
  id: string;
  expenseId: string;
  action: 'Submitted' | 'Approved' | 'Rejected';
  performedBy: string;
  timestamp: string;
  notes?: string;
  expense?: ManagerExpense; // optional reference for UI history details
}

export interface ManagerDashboard {
  pendingRequestsCount: number;
  approvedTodayCount: number;
  rejectedTodayCount: number;
  totalPendingAmount: number;
  recentPendingRequests: ManagerExpense[];
}
