import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = '';
  password = '';
  error = '';
  loading = false;

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit(): void {
    this.error = '';
    this.loading = true;

    this.auth.login(this.username, this.password).subscribe({
      next: (response) => {
        this.loading = false;

        if (response.success) {
          if (response.role === 'Manager') {
            this.router.navigateByUrl('/manager');
          } else if (response.role === 'Accountant') {
            this.router.navigateByUrl('/accountant');
          } else if (response.role === 'Admin') {
            this.router.navigateByUrl('/admin');
          } else {
            this.router.navigateByUrl('/user');
          }
        } else {
          this.error = response.message;
        }
      },
      error: (err: Error) => {
        this.loading = false;
        this.error = err.message;
      }
    });
  }
}
