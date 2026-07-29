import { Component, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent {
  readonly displayName = computed(() => this.auth.currentUserName() ?? 'Himeshwar');

  constructor(public auth: AuthService) {}
}
