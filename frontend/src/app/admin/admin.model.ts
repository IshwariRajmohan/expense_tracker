import { ExpenseItem, UserProfile } from '../employee/employee.model';

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

export interface AdminUser {
  name: string;
  email: string;
  role: string;
  department: string;
  employeeId: string;
  budgetLimit: number;
  spentAmount: number;
  avatarUrl: string;
}

export interface AdminExpense {
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

export interface ChartDataPoint {
  label: string;
  value: number;
}

export interface StatusChartDataPoint {
  status: string;
  count: number;
  amount: number;
}

export interface ActivityLog {
  id: string;
  action: string;
  timestamp: string;
  statusType: string;
}

export interface AdminDashboard {
  totalEmployees: number;
  totalManagers: number;
  totalAccountants: number;
  totalExpenses: number;
  pendingCount: number;
  approvedCount: number;
  rejectedCount: number;
  paidCount: number;
  totalExpenseAmount: number;
  monthlyExpenseChartData: ChartDataPoint[];
  statusChartData: StatusChartDataPoint[];
  recentActivities: ActivityLog[];
}

export interface DepartmentReport {
  departmentName: string;
  count: number;
  totalAmount: number;
}

export interface EmployeeReport {
  employeeName: string;
  employeeId: string;
  role: string;
  department: string;
  count: number;
  totalAmount: number;
}

export interface AdminReports {
  monthlyExpenseReport: ChartDataPoint[];
  departmentWiseExpenses: DepartmentReport[];
  employeeWiseExpenses: EmployeeReport[];
  statusWiseExpenses: StatusChartDataPoint[];
  topSpendingEmployees: EmployeeReport[];
}

export interface AdminFreezeDate {
  freezeDay: number;
  isClosed: boolean;
  currentMonth: string;
}

export interface AdminSettings {
  companyName: string;
  companyAddress: string;
  corporateCurrency: string;
  systemMode: string;
  adminProfile: UserProfile;
}
