import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EntrepriseService } from '../../services/entreprise.service';
import { Entreprise } from '../../models/entreprise.model';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../notification-toast/notification-toast.component';

@Component({
  selector: 'app-entreprises',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationToastComponent],
  templateUrl: './entreprises.component.html',
  styleUrls: ['./entreprises.component.scss']
})
export class EntreprisesComponent implements OnInit {
  entreprises: Entreprise[] = [];
  isLoading = false;
  searchTerm = '';
  includeInactive = false;
  //   Mode d'affichage : table compacte OU cartes (mêmes choix que la page Prestataires)
  viewMode: 'list' | 'cards' = 'cards';
  toggleViewMode(mode: 'list' | 'cards'): void { this.viewMode = mode; }

  //   Initiales pour l'avatar (ex : "CAP" → "CA", "Capgemini" → "CA")
  getInitials(e: Entreprise): string {
    const src = e.code || e.nom || '';
    return src.substring(0, 2).toUpperCase();
  }

  //   Modal édition / création
  isModalOpen = false;
  isEditMode = false;
  form: { id: string | null; nom: string; code: string; description: string; isActive: boolean } = {
    id: null, nom: '', code: '', description: '', isActive: true
  };

  //   Confirmation suppression
  showDeleteConfirm = false;
  toDelete: Entreprise | null = null;

  constructor(
    private entrepriseService: EntrepriseService,
    private notifService: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.entrepriseService.getAll(this.includeInactive).subscribe({
      next: (list) => {
        this.entreprises = list ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.notifService.show('error', 'Erreur lors du chargement des entreprises.');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  //   Filtrage local (recherche par nom / code)
  get filtered(): Entreprise[] {
    const q = this.searchTerm.trim().toLowerCase();
    if (!q) return this.entreprises;
    return this.entreprises.filter(e =>
      (e.nom || '').toLowerCase().includes(q)
      || (e.code || '').toLowerCase().includes(q)
    );
  }

  // ── Modal ──────────────────────────────────────────────────────────────
  openCreate(): void {
    this.isEditMode = false;
    this.form = { id: null, nom: '', code: '', description: '', isActive: true };
    this.isModalOpen = true;
  }

  openEdit(e: Entreprise): void {
    this.isEditMode = true;
    this.form = {
      id: e.id,
      nom: e.nom,
      code: e.code ?? '',
      description: e.description ?? '',
      isActive: e.isActive
    };
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  save(): void {
    const nom = this.form.nom?.trim();
    if (!nom) {
      this.notifService.show('warning', 'Le nom est obligatoire.');
      return;
    }
    const payload = {
      nom,
      code: this.form.code?.trim() || null,
      description: this.form.description?.trim() || null
    };
    if (this.isEditMode && this.form.id) {
      this.entrepriseService.update(this.form.id, { ...payload, isActive: this.form.isActive })
        .subscribe({
          next: () => {
            this.notifService.show('success', 'Entreprise modifiée.');
            this.closeModal();
            this.load();
          },
          error: () => this.notifService.show('error', 'Erreur lors de la modification.')
        });
    } else {
      this.entrepriseService.create(payload).subscribe({
        next: () => {
          this.notifService.show('success', 'Entreprise créée.');
          this.closeModal();
          this.load();
        },
        error: () => this.notifService.show('error', 'Erreur lors de la création.')
      });
    }
  }

  // ── Suppression ─────────────────────────────────────────────────────────
  confirmDelete(e: Entreprise): void {
    this.toDelete = e;
    this.showDeleteConfirm = true;
  }
  cancelDelete(): void {
    this.showDeleteConfirm = false;
    this.toDelete = null;
  }
  doDelete(): void {
    if (!this.toDelete?.id) return;
    this.entrepriseService.delete(this.toDelete.id).subscribe({
      next: () => {
        this.notifService.show('success', 'Entreprise supprimée.');
        this.showDeleteConfirm = false;
        this.toDelete = null;
        this.load();
      },
      error: () => {
        this.notifService.show('error', 'Erreur lors de la suppression.');
        this.showDeleteConfirm = false;
        this.toDelete = null;
      }
    });
  }
}
