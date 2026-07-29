import { Routes } from '@angular/router';
import { LoginComponent } from './login.component';
import { EmployeeComponent } from './employee/employee.component';
import { ManagerComponent } from './manager/manager.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'user', component: EmployeeComponent },
  { path: 'employee', component: EmployeeComponent },
  { path: 'manager', component: ManagerComponent },
  { path: '**', redirectTo: '' }
];
