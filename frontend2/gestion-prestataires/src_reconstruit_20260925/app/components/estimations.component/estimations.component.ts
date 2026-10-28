import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EstimationService } from '../../services/estimation.service';
import { Estimation } from '../../models/estimation.model';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';
import { PermissionService, Role } from '../../services/permission.service';

@Component({
  selector: 'app-estimations',
  standalone: true,
  imports: [CommonModule, FormsModule, FormatMontantPipe],
  templateUrl: './estimations.component.html',
  styleUrls: ['./estimations.component.scss']
})
export class EstimationsComponent implements OnInit {
  estimations: Estimation[] = [];
  filteredEstimations: Estimation[] = [];
  isLoading = false;
  viewMode: 'list' | 'cards' = 'cards';

  // Filtres
  searchTerm = '';
  filterAnnee = '';
  anneesUniques: number[] = [];

  // Notification
  notification = {
    show: false,
    type: 'success' as 'success' | 'error' | 'warning',
    message: ''
  };

  // Confirmation suppression
  showDeleteConfirm = false;
  estimationToDelete: Estimation | null = null;

  constructor(
    private estimationService: EstimationService,
    private router: Router,
    private permissionService: PermissionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // Vérifier que c'est un Admin
    if (!this.permissionService.hasRole(Role.Admin)) {
      this.showNotification('error', 'Accès refusé : réservé aux Admins');
      this.router.navigate(['/dashboard']);
      return;
    }

    this.loadEstimations();
  }

  loadEstimations() {
    this.isLoading = true;
    this.estimationService.getAll().subscribe({
      next: (data) => {
        this.estimations = data;
        this.filteredEstimations = data;
        this.extractAnneesUniques();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Erreur chargement estimations:', err);
        this.showNotification('error', 'Erreur lors du chargement des estimations');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  extractAnneesUniques() {
    const annees = this.estimations.map(e => e.annee);
    this.anneesUniques = [...new Set(annees)].sort((a, b) => b - a);
  }

  applyFilters() {
    this.filteredEstimations = this.estimations.filter(e => {
      const matchSearch = !this.searchTerm || 
        e.annee.toString().includes(this.searchTerm) ||
        (e.createurNom && e.createurNom.toLowerCase().includes(this.searchTerm.toLowerCase()));
      
      const matchAnnee = !this.filterAnnee || e.annee.toString() === this.filterAnnee;
      
      return matchSearch && matchAnnee;
    });
  }

  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }

  navigateToDetail(id?: string) {
    if (id) {
      this.router.navigate(['/estimations', id, 'detail']);
    } else {
      this.router.navigate(['/estimations', 'nouveau']);
    }
  }

  navigateToResultat(id: string) {
    this.router.navigate(['/estimations', id, 'resultat']);
  }

  confirmDelete(estimation: Estimation) {
    this.estimationToDelete = estimation;
    this.showDeleteConfirm = true;
    this.cdr.detectChanges();
  }

  deleteEstimation() {
    if (this.estimationToDelete && this.estimationToDelete.id) {
      this.estimationService.delete(this.estimationToDelete.id).subscribe({
        next: () => {
          this.showNotification('success', 'Estimation supprimée avec succès');
          this.loadEstimations();
          this.showDeleteConfirm = false;
          this.estimationToDelete = null;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Erreur suppression:', err);
          this.showNotification('error', 'Erreur lors de la suppression');
          this.showDeleteConfirm = false;
          this.estimationToDelete = null;
          this.cdr.detectChanges();
        }
      });
    }
  }

  cancelDelete() {
    this.showDeleteConfirm = false;
    this.estimationToDelete = null;
  }

  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notification = { show: true, type, message };
    setTimeout(() => {
      this.notification.show = false;
      this.cdr.detectChanges();
    }, 5000);
  }

  getBudgetRestant(estimation: Estimation): number {
    return estimation.budgetCagnotte - estimation.budgetConsomme;
  }

  getTotalRevenu(estimation: Estimation): number {
    return estimation.lignes.reduce((sum, l) => sum + l.revenu, 0);
  }

  getPourcentageUtilise(estimation: Estimation): number {
    if (estimation.budgetCagnotte === 0) return 0;
    return (estimation.budgetConsomme / estimation.budgetCagnotte) * 100;
  }

  formatDate(date: Date | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('fr-FR');
  }
}