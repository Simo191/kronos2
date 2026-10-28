import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TypeContratService, TypeContratDto } from '../../services/type-contrat.service';

@Component({
  selector: 'app-types-contrat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './types-contrat.component.html',
  styleUrls: ['./types-contrat.component.scss']
})
export class TypesContratComponent implements OnInit {
  typesContrat: TypeContratDto[] = [];
  filteredTypes: TypeContratDto[] = [];
  selectedType: Partial<TypeContratDto> = {};
  
  isModalOpen = false;
  isEditMode = false;
  isLoading = false;
  viewMode: 'list' | 'cards' = 'cards';
  
  // Filtres
  searchTerm = '';
  sortBy = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 9;
  
  // Statistiques
  totalTypes = 0;
  typesWithDescription = 0;
  
  // Notification
  notification = {
    show: false,
    type: 'success' as 'success' | 'error' | 'warning',
    message: ''
  };
  
  // Confirmation suppression
  showDeleteConfirm = false;
  typeToDelete: TypeContratDto | null = null;

  constructor(
    private typeContratService: TypeContratService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadTypesContrat();
  }

  loadTypesContrat() {
    this.isLoading = true;
    this.typeContratService.getAll().subscribe({
      next: (data) => {
        this.typesContrat = data ?? [];
        this.filteredTypes = [...this.typesContrat];
        this.calculateStats();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du chargement des types');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Calcul des statistiques
  calculateStats() {
    this.totalTypes = this.typesContrat.length;
    this.typesWithDescription = this.typesContrat.filter(t => t.description && t.description.trim() !== '').length;
  }

  // Application des filtres
  applyFilters() {
    this.filteredTypes = this.typesContrat.filter(type => {
      const nom = type.libelle || type.libelle || '';
      const description = type.description || '';
      
      const searchMatch = !this.searchTerm || 
        nom.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        description.toLowerCase().includes(this.searchTerm.toLowerCase());

      return searchMatch;
    });

    this.applySorting();
    this.currentPage = 1;
  }

  // Application du tri
  applySorting() {
    if (this.sortBy === 'nom') {
      this.filteredTypes.sort((a, b) => {
        const nomA = a.libelle || a.libelle || '';
        const nomB = b.libelle || b.libelle || '';
        return nomA.localeCompare(nomB);
      });
    } else if (this.sortBy === 'recent') {
      this.filteredTypes.reverse();
    }
  }

  // Modal
  openModal(type?: TypeContratDto) {
    this.isEditMode = !!type;
    if (type) {
      this.selectedType = { 
        ...type,
        libelle: type.libelle || type.libelle
      };
    } else {
      this.selectedType = {};
    }
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedType = {};
  }

  saveType() {
    // Validation
    if (!this.selectedType.libelle && !this.selectedType.libelle) {
      this.showNotification('warning', 'Veuillez saisir un nom');
      return;
    }

    // S'assurer que libelle est défini pour l'API
    if (!this.selectedType.libelle) {
      this.selectedType.libelle = this.selectedType.libelle;
    }

    if (this.isEditMode && this.selectedType.id) {
      this.typeContratService.update(this.selectedType.id, this.selectedType as TypeContratDto).subscribe({
        next: () => {
          this.loadTypesContrat();
          this.closeModal();
          this.showNotification('success', 'Type modifié avec succès !');
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la modification');
        }
      });
    } else {
      this.typeContratService.create(this.selectedType as TypeContratDto).subscribe({
        next: () => {
          this.loadTypesContrat();
          this.closeModal();
          this.showNotification('success', 'Type créé avec succès !');
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la création');
        }
      });
    }
  }

  // Confirmation de suppression
  confirmDelete(type: TypeContratDto) {
    this.typeToDelete = type;
    this.showDeleteConfirm = true;
  }

  deleteType() {
    if (this.typeToDelete && this.typeToDelete.id) {
      this.typeContratService.delete(this.typeToDelete.id).subscribe({
        next: () => {
          this.loadTypesContrat();
          this.showDeleteConfirm = false;
          this.typeToDelete = null;
          this.showNotification('success', 'Type supprimé avec succès !');
        },
        error: () => {
          this.showNotification('error', 'Erreur lors de la suppression');
          this.showDeleteConfirm = false;
          this.typeToDelete = null;
        }
      });
    }
  }

  // Notification
  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notification = { show: true, type, message };
    setTimeout(() => {
      this.notification.show = false;
      this.cdr.detectChanges();
    }, 5000);
  }

  // Pagination
  get totalPages(): number {
    return Math.ceil(this.filteredTypes.length / this.pageSize);
  }

  get pagedTypes(): TypeContratDto[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredTypes.slice(start, start + this.pageSize);
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  get startItemIndex(): number {
    return this.filteredTypes.length === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get endItemIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.filteredTypes.length);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  // Utilitaires
  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }
}