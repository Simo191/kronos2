import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
 import { Permission, PermissionService, Role } from '../../services/permission.service';
import { MoisVerrouilleDto, MoisVerrouilleService } from '../../services/verrouille.service';
import { NotificationService } from '../../services/notification.service';
import { NotificationToastComponent } from '../../components/notification-toast/notification-toast.component';

@Component({
  selector: 'app-gestion-verrouillages',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationToastComponent],
  templateUrl: './gestion-verrouillages.component.html',
  styleUrls: ['./gestion-verrouillages.component.scss']
})
export class GestionVerrouillageSComponent implements OnInit {
  moisVerrouilles: MoisVerrouilleDto[] = [];
  isModalOpen = false;
  isLoading = false;

  // Formulaire
  selectedYear = new Date().getFullYear();
  selectedMonth = '';
  commentaire = '';

  // Month picker
  showMonthPicker = false;
  moisList = [
    { value: 1, label: 'Janvier' },
    { value: 2, label: 'Février' },
    { value: 3, label: 'Mars' },
    { value: 4, label: 'Avril' },
    { value: 5, label: 'Mai' },
    { value: 6, label: 'Juin' },
    { value: 7, label: 'Juillet' },
    { value: 8, label: 'Août' },
    { value: 9, label: 'Septembre' },
    { value: 10, label: 'Octobre' },
    { value: 11, label: 'Novembre' },
    { value: 12, label: 'Décembre' }
  ];


  // Confirmation suppression
  showDeleteConfirm = false;
  moisToDelete: MoisVerrouilleDto | null = null;

  constructor(
    private moisVerrouilleService: MoisVerrouilleService,
    private permissionService: PermissionService,
    private cdr: ChangeDetectorRef,
    private notifService: NotificationService
  ) {}

  ngOnInit() {
    // Vérifier les permissions
    if (!this.canManage()) {
      this.showNotification('error', 'Accès refusé : réservé aux Admin/Manager');
      return;
    }

    this.loadMoisVerrouilles();
  }

  canManage(): boolean {
    return this.permissionService.hasRole(Role.Admin) || this.permissionService.hasRole(Role.Manager);
  }

  loadMoisVerrouilles() {
    this.isLoading = true;
    this.moisVerrouilleService.getAll().subscribe({
      next: (data) => {
        this.moisVerrouilles = data.sort((a, b) => b.mois.localeCompare(a.mois));
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.showNotification('error', 'Erreur lors du chargement des mois verrouillés');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openModal() {
    this.selectedYear = new Date().getFullYear();
    this.selectedMonth = '';
    this.commentaire = '';
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedMonth = '';
    this.commentaire = '';
    this.showMonthPicker = false;
  }

  verrouiller() {
    if (!this.selectedMonth) {
      this.showNotification('warning', 'Veuillez sélectionner un mois');
      return;
    }

    this.moisVerrouilleService.verrouiller(this.selectedMonth, this.commentaire).subscribe({
      next: () => {
        this.closeModal();
        this.loadMoisVerrouilles();
        this.showNotification('success', `Mois ${this.selectedMonth} verrouillé avec succès !`);
      },
      error: (err) => {
        const errorMsg = err?.error?.message || 'Erreur lors du verrouillage';
        this.showNotification('error', errorMsg);
      }
    });
  }

  confirmDeverrouiller(mois: MoisVerrouilleDto) {
    this.moisToDelete = mois;
    this.showDeleteConfirm = true;
    this.cdr.detectChanges();
  }

  deverrouiller() {
    if (this.moisToDelete && this.moisToDelete.id) {
      this.moisVerrouilleService.deverrouiller(this.moisToDelete.id).subscribe({
        next: () => {
          this.loadMoisVerrouilles();
          this.showDeleteConfirm = false;
          this.moisToDelete = null;
          this.showNotification('success', 'Mois déverrouillé avec succès !');
          this.cdr.detectChanges();
        },
        error: () => {
          this.showNotification('error', 'Erreur lors du déverrouillage');
          this.showDeleteConfirm = false;
          this.moisToDelete = null;
          this.cdr.detectChanges();
        }
      });
    }
  }

  cancelDelete() {
    this.showDeleteConfirm = false;
    this.moisToDelete = null;
  }

  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notifService.show(type, message);
  }

  formatMois(mois: string): string {
    const [annee, moisNum] = mois.split('-');
    const date = new Date(parseInt(annee), parseInt(moisNum) - 1, 1);
    return date.toLocaleDateString('fr-FR', { year: 'numeric', month: 'long' });
  }

  // Month Picker
  toggleMonthPicker(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.showMonthPicker = !this.showMonthPicker;

    if (this.showMonthPicker) {
      setTimeout(() => {
        const closeHandler = (e: Event) => {
          const target = e.target as HTMLElement;
          if (!target.closest('.custom-month-picker')) {
            this.closeMonthPicker();
            document.removeEventListener('click', closeHandler);
          }
        };
        document.addEventListener('click', closeHandler);
      }, 0);
    }
  }

  closeMonthPicker() {
    this.showMonthPicker = false;
    this.cdr.detectChanges();
  }

  changeYear(delta: number, event?: Event) {
    if (event) {
      event.stopPropagation();
      event.preventDefault();
    }
    this.selectedYear += delta;
  }

  selectMonth(monthNumber: number, event?: Event) {
    if (event) {
      event.stopPropagation();
      event.preventDefault();
    }
    
    const monthStr = String(monthNumber).padStart(2, '0');
    this.selectedMonth = `${this.selectedYear}-${monthStr}`;
    this.showMonthPicker = false;
    this.cdr.detectChanges();
  }

  isMonthSelected(monthNumber: number): boolean {
    if (!this.selectedMonth) return false;
    const [year, month] = this.selectedMonth.split('-');
    return parseInt(year) === this.selectedYear && parseInt(month) === monthNumber;
  }

  formatMoisInput(mois: string): string {
    if (!mois) return '';
    const [year, month] = mois.split('-');
    const monthName = this.moisList.find(m => m.value === parseInt(month))?.label || '';
    return `${monthName} ${year}`;
  }

  // ✅ Vérifier si un mois est déjà verrouillé
  isMoisVerrouille(mois: string): boolean {
    return this.moisVerrouilles.some(m => m.mois === mois);
  }
}