import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="unauthorized-container">
      <div class="unauthorized-content">
        <div class="icon-container">
          <i class="bi bi-shield-exclamation"></i>
        </div>
        
        <h1 class="title">Accès Refusé</h1>
        
        <p class="message">
          Vous n'avez pas les permissions nécessaires pour accéder à cette page.
        </p>
        
        <p class="sub-message">
          Si vous pensez qu'il s'agit d'une erreur, veuillez contacter votre administrateur.
        </p>
        
        <div class="actions">
          <button class="btn btn-primary" (click)="goToDashboard()">
            <i class="bi bi-house-door"></i>
            Retour au Dashboard
          </button>
          <button class="btn btn-secondary" (click)="goBack()">
            <i class="bi bi-arrow-left"></i>
            Page Précédente
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .unauthorized-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }

    .unauthorized-content {
      background: white;
      border-radius: 20px;
      padding: 3rem;
      max-width: 500px;
      width: 100%;
      text-align: center;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
      animation: slideUp 0.5s ease-out;
    }

    @keyframes slideUp {
      from {
        opacity: 0;
        transform: translateY(30px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    .icon-container {
      width: 120px;
      height: 120px;
      margin: 0 auto 2rem;
      background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      animation: pulse 2s ease-in-out infinite;

      i {
        font-size: 4rem;
        color: #dc2626;
      }
    }

    @keyframes pulse {
      0%, 100% {
        transform: scale(1);
      }
      50% {
        transform: scale(1.05);
      }
    }

    .title {
      font-size: 2rem;
      font-weight: 700;
      color: #1f2937;
      margin: 0 0 1rem;
    }

    .message {
      font-size: 1.1rem;
      color: #6b7280;
      margin: 0 0 0.5rem;
      line-height: 1.6;
    }

    .sub-message {
      font-size: 0.9rem;
      color: #9ca3af;
      margin: 0 0 2rem;
    }

    .actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;

      .btn {
        padding: 0.75rem 1.5rem;
        border: none;
        border-radius: 10px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
        display: flex;
        align-items: center;
        gap: 0.5rem;

        i {
          font-size: 1.1rem;
        }

        &.btn-primary {
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;

          &:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
          }
        }

        &.btn-secondary {
          background: #f3f4f6;
          color: #374151;

          &:hover {
            background: #e5e7eb;
          }
        }
      }
    }

    @media (max-width: 768px) {
      .unauthorized-content {
        padding: 2rem;
      }

      .title {
        font-size: 1.5rem;
      }

      .actions {
        flex-direction: column;

        .btn {
          width: 100%;
          justify-content: center;
        }
      }
    }
  `]
})
export class UnauthorizedComponent {
  constructor(private router: Router) {}

  goToDashboard() {
    this.router.navigate(['/dashboard']);
  }

  goBack() {
    window.history.back();
  }
}