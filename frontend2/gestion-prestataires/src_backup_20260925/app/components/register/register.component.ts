import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  email = '';
  password = '';
  confirmPassword = '';
  firstName = '';
  lastName = '';
  errorMessage = '';
  successMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit() {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.email || !this.password || !this.firstName || !this.lastName) {
      this.errorMessage = 'Veuillez remplir tous les champs';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Les mots de passe ne correspondent pas';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'Le mot de passe doit contenir au moins 6 caractères';
      return;
    }

    this.isLoading = true;

    this.authService.register({
      email: this.email,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName
    }).subscribe({
      next: () => {
        this.successMessage = 'Inscription réussie ! Connexion...';
        setTimeout(() => {
          this.authService.login({ email: this.email, password: this.password })
            .subscribe({
              next: () => this.router.navigate(['/dashboard']),
              error: () => this.router.navigate(['/login'])
            });
        }, 1500);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Erreur lors de l\'inscription';
        this.isLoading = false;
      }
    }); this.isLoading = false;
  }
}
