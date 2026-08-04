import { ExpenseItem } from '../employee/employee.model';

export interface EmployeeInfo {
  name: string;
  email: string;
  employeeId: string;
  department: string;
  avatarUrl: string;
}

export interface ApprovalHistoryItem {
  id: string;
  expenseId: string;
  action: string;
  performedBy: string;
  timestamp: string;
  notes: string;
}

export interface AccountantPaymentActivity {
  expenseId: string;
  employeeName: string;
  totalAmount: number;
  paymentDate: string;
}

export interface AccountantDashboard {
  approvedExpensesCount: number;
  paidExpensesCount: number;
  totalAmountToPay: number;
  totalAmountPaid: number;
  recentPaymentActivities: AccountantPaymentActivity[];
}

export interface AccountantExpense {
  id: string;
  title: string;
  category: string;
  date: string;
  description: string;
  items: ExpenseItem[];
  totalAmount: number;
  status: string;
  notes?: string;
  paymentDate?: string;
  employee: EmployeeInfo;
  approvalHistory: ApprovalHistoryItem[];
}
