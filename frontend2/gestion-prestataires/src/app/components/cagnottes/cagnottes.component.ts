import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CagnotteService } from '../../services/cagnotte.service';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';
import { Cagnotte, SourceBudget } from '../../models/cagnotte.model';

@Component({
  selector: 'app-cagnottes',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationToastComponent],
  templateUrl: './cagnottes.component.html',
  styleUrls: ['./cagnottes.component.scss']
})
export class CagnottesComponent implements OnInit {
  cagnottes: Cagnotte[] = [];
  loading = false;
  showModal = false;
  showSourceModal = false;
  isEditMode = false;
  isEditSourceMode = false;
  viewMode: 'list' | 'cards' = 'cards';
  currentYear = new Date().getFullYear();

  currentCagnotte: Partial<Cagnotte> = {
    annee: new Date().getFullYear(),
    sourcesBudget: []
  };

  currentSource: Partial<SourceBudget> = {
    nom: '',
    code: '',
    montant: 0,
    description: '',
    cagnotteId: ''
  };

  selectedCagnotteForSource: Cagnotte | null = null;

  // ✅ NOUVEAU : Confirmation suppression
  showDeleteConfirm = false;
  cagnotteToDelete: Cagnotte | null = null;

  // Statistiques
  totalCagnottes = 0;
  budgetTotal = 0;
  budgetConsommeTotal = 0;


  constructor(
    private cagnotteService: CagnotteService,
    private cdr: ChangeDetectorRef,
    private notifService: NotificationService
  ) {}

  ngOnInit() {
    this.loadCagnottes();
  }

  loadCagnottes() {
    this.loading = true;
    this.cagnotteService.getAll().subscribe({
      next: (data) => {
        this.cagnottes = data ?? [];
        this.calculateStats();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.showNotification('error', 'Erreur lors du chargement des budgets');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Calcul des statistiques
  calculateStats() {
    this.totalCagnottes = this.cagnottes.length;
    this.budgetTotal = this.cagnottes.reduce((sum, c) => sum + (c.budgetAnnuel || 0), 0);
    this.budgetConsommeTotal = this.cagnottes.reduce((sum, c) => sum + (c.budgetConsomme || 0), 0);
  }

  // Modal Cagnotte
  openModal() {
    this.isEditMode = false;
    this.currentCagnotte = {
      annee: new Date().getFullYear(),
      sourcesBudget: []
    };
    this.showModal = true;
  }

  openEditModal(cagnotte: Cagnotte) {
    this.isEditMode = true;
    this.currentCagnotte = { ...cagnotte };
    if (!this.currentCagnotte.sourcesBudget) {
      this.currentCagnotte.sourcesBudget = [];
    }
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.currentCagnotte = {
      annee: new Date().getFullYear(),
      sourcesBudget: []
    };
  }

  saveCagnotte() {    
    if (!this.currentCagnotte.annee) {
      this.showNotification('warning', 'Veuillez remplir tous les champs obligatoires');
      return;
    }

    if (!this.currentCagnotte.sourcesBudget || this.currentCagnotte.sourcesBudget.length === 0) {
      this.showNotification('warning', 'Veuillez ajouter au moins une source de budget');
      return;
    }

    if (this.isEditMode && this.currentCagnotte.id) {      
      this.cagnotteService.update(this.currentCagnotte.id, this.currentCagnotte).subscribe({
        next: () => {
          this.closeModal();
          this.loadCagnottes();
          this.cdr.detectChanges();
          this.showNotification('success', 'Budget mise à jour avec succès !');
        },
        error: (err) => {
          console.error('❌ Erreur de mise à jour:', err);
          this.showNotification('error', err.error?.message || 'Erreur lors de la mise à jour');
        }
      });
    } else {      
      this.cagnotteService.create(this.currentCagnotte).subscribe({
        next: () => {
          this.closeModal();
          this.loadCagnottes();
          this.cdr.detectChanges();
          this.showNotification('success', 'Budget créée avec succès !');
        },
        error: (err) => {
          this.showNotification('error', err.error?.message || 'Erreur lors de la création');
        }
      });
    }
  }

  // ✅ NOUVEAU : Suppression
  confirmDelete(cagnotte: Cagnotte) {
    this.cagnotteToDelete = cagnotte;
    this.showDeleteConfirm = true;
  }

  cancelDelete() {
    this.showDeleteConfirm = false;
    this.cagnotteToDelete = null;
  }

  deleteCagnotte() {
    if (!this.cagnotteToDelete || !this.cagnotteToDelete.id) {
      console.error('❌ Aucune budget à supprimer');
      return;
    }

    const cagnotteId = this.cagnotteToDelete.id;
    const cagnotteAnnee = this.cagnotteToDelete.annee;

    this.cagnotteService.delete(cagnotteId).subscribe({
      next: () => {
        this.showNotification('success', `Cudget ${cagnotteAnnee} supprimée avec succès !`);
        this.loadCagnottes();
        this.cancelDelete();
      },
      error: (err) => {
        console.error('❌ Erreur lors de la suppression:', err);
        this.showNotification('error', err.error?.message || 'Erreur lors de la suppression');
        this.cancelDelete();
      }
    });
  }

  // Modal Source de Budget
  openSourceModal(cagnotte?: Cagnotte) {
    this.isEditSourceMode = false;
    this.currentSource = {
      nom: '',
      code: '',
      montant: 0,
      description: '',
      cagnotteId: cagnotte?.id || this.currentCagnotte.id || ''
    };
    this.selectedCagnotteForSource = cagnotte || null;
    this.showSourceModal = true;
  }

  openEditSourceModal(source: SourceBudget, cagnotte?: Cagnotte) {
    this.isEditSourceMode = true;
    this.currentSource = { ...source };
    this.selectedCagnotteForSource = cagnotte || null;
    this.showSourceModal = true;
  }

  closeSourceModal() {
    this.showSourceModal = false;
    this.currentSource = {
      nom: '',
      code: '',
      montant: 0,
      description: '',
      cagnotteId: ''
    };
  }

  addSourceToCurrentCagnotte() {
    if (!this.currentSource.nom || !this.currentSource.code || !this.currentSource.montant) {
      this.showNotification('warning', 'Veuillez remplir tous les champs obligatoires');
      return;
    }

    if (!this.currentCagnotte.sourcesBudget) {
      this.currentCagnotte.sourcesBudget = [];
    }

    if (this.isEditSourceMode) {
      const index = this.currentCagnotte.sourcesBudget.findIndex(s => s.id === this.currentSource.id);
      if (index !== -1) {
        this.currentCagnotte.sourcesBudget[index] = { ...this.currentSource } as SourceBudget;
      }
    } else {
      const newSource: SourceBudget = {
        nom: this.currentSource.nom!,
        code: this.currentSource.code!,
        montant: this.currentSource.montant!,
        description: this.currentSource.description,
        cagnotteId: this.currentCagnotte.id || ''
      };
      this.currentCagnotte.sourcesBudget.push(newSource);
    }

    this.closeSourceModal();
    this.showNotification('success', 'Source de budget ajoutée !');
  }

  removeSourceFromCurrentCagnotte(source: SourceBudget) {
    if (this.currentCagnotte.sourcesBudget) {
      this.currentCagnotte.sourcesBudget = this.currentCagnotte.sourcesBudget.filter(s => s.id !== source.id);
      this.showNotification('success', 'Source de budget supprimée !');
    }
  }

  calculateTotalSources(): number {
    if (!this.currentCagnotte.sourcesBudget) return 0;
    return this.currentCagnotte.sourcesBudget.reduce((sum, s) => sum + (s.montant || 0), 0);
  }

  // Recalculer
  recalculer(annee: number) {
    this.cagnotteService.recalculer(annee).subscribe({
      next: () => {
        this.loadCagnottes();
        this.showNotification('success', 'Budget recalculé avec succès !');
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du recalcul');
      }
    });
  }

  // Notification
  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notifService.show(type, message);
  }

  // Utilitaires
  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }

  getBudgetClass(budgetRestant: number): string {
    return budgetRestant >= 0 ? 'text-success' : 'text-danger';
  }

  getProgressPercent(cagnotte: Cagnotte): number {
    if (!cagnotte.budgetAnnuel) return 0;
    return Math.min((cagnotte.budgetConsomme / cagnotte.budgetAnnuel) * 100, 100);
  }

  formatMontant(montant: number): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(montant);
  }
}