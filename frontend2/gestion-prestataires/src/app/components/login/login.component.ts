import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LogoPrestaPro } from '../logo-prestapro/logo-prestapro.component'; // ✅ IMPORT

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, LogoPrestaPro], // ✅ AJOUT
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';
  isLoading = false;
  rememberMe = false;

  constructor(
    private authService: AuthService,
    private router: Router,    private cdr: ChangeDetectorRef

  ) {}

  onSubmit() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Veuillez remplir tous les champs';
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
  
    this.authService.login({ 
      email: this.email, 
      password: this.password,
      rememberMe: this.rememberMe   // ✅ NOUVEAU
    }).subscribe({
      next: () => { this.router.navigate(['/dashboard']); },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Erreur lors de la connexion';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }
}