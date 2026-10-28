import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EstimationService } from '../../services/estimation.service';
import { PrestataireService } from '../../services/prestataire.service';
import { CagnotteService } from '../../services/cagnotte.service';
import { Estimation, CreateEstimationCommand, UpdateEstimationCommand } from '../../models/estimation.model';
import { Prestataire } from '../../models/prestataire.model';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';

// ✅ FORMAT MATRICIEL : Une ligne par prestataire, 12 colonnes pour les mois
interface PrestataireRow {
  prestataireId: string | null;
  nomPrestataire: string;
  nomFuturPrestataire: string | null;
  useFuturePrestataire: boolean;
  tjm: number;
  moisData: { [mois: number]: { jours: number } };  // 1-12
  totalJours: number;
  totalRevenu: number;
  ordre: number;
}

@Component({
  selector: 'app-estimation-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, FormatMontantPipe],
  templateUrl: './estimation-detail.component.html',
  styleUrls: ['./estimation-detail.component.scss']
})
export class EstimationDetailComponent implements OnInit {
  estimationId?: string;
  isEditMode = false;
  isLoading = false;

  annee: number = new Date().getFullYear();
  budgetCagnotte: number = 0;
  budgetConsomme: number = 0;

  // ✅ TABLEAU MATRICIEL
  prestataireRows: PrestataireRow[] = [];

  prestataires: Prestataire[] = [];
  moisList = [
    { value: 1, label: 'JAN' },
    { value: 2, label: 'FÉV' },
    { value: 3, label: 'MAR' },
    { value: 4, label: 'AVR' },
    { value: 5, label: 'MAI' },
    { value: 6, label: 'JUIN' },
    { value: 7, label: 'JUIL' },
    { value: 8, label: 'AOÛT' },
    { value: 9, label: 'SEP' },
    { value: 10, label: 'OCT' },
    { value: 11, label: 'NOV' },
    { value: 12, label: 'DÉC' }
  ];

  notification = {
    show: false,
    type: 'success' as 'success' | 'error' | 'warning',
    message: ''
  };

  loadingMessage = 'Chargement en cours...';

  // ✅ Collapse Informations générales
  infoCollapsed = false;

  // ✅ Modal confirmation cagnotte
  showCagnotteConfirm = false;
  cagnotteConfirmBudget = 0;
  cagnotteConfirmAnnee = 0;

  // ✅ Drag & drop lignes
  draggedIndex: number | null = null;
  dragOverIndex: number | null = null;

  // ✅ Confirmation suppression
  showDeleteConfirm = false;
  deleteTargetIndex: number | null = null;

  constructor(
    private estimationService: EstimationService,
    private prestataireService: PrestataireService,
    private cagnotteService: CagnotteService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadPrestataires();
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id && id !== 'nouveau') {
        this.estimationId = id;
        this.isEditMode = true;
        this.loadEstimation(id);
      } else {
        this.ajouterPrestataire();
      }
    });
  }

  loadPrestataires() {
    this.prestataireService.getAll().subscribe({
      next: (data) => {
        this.prestataires = data.filter(p => p.responsable !== true);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Erreur:', err)
    });
  }

  loadEstimation(id: string) {
    this.isLoading = true;
    this.loadingMessage = 'Chargement de l\'estimation...';
    this.estimationService.getById(id).subscribe({
      next: (data) => {
        this.annee = data.annee;
        this.budgetCagnotte = data.budgetCagnotte;
        this.convertirEnMatriciel(data.lignes);
        this.calculerBudgetConsomme();
        this.isLoading = false;
        this.cdr.detectChanges();
       },
      error: (err) => {
        console.error('Erreur:', err);
        this.showNotification('error',err);
        this.isLoading = false;
      }
    });
  }

  // ✅ Déclenché quand l'année change : chercher la cagnotte correspondante
  onAnneeChange() {
    if (!this.annee || this.isEditMode) return;
    this.cagnotteService.getByAnnee(this.annee).subscribe({
      next: (cagnotte) => {
        if (cagnotte && cagnotte.budgetAnnuel) {
          this.cagnotteConfirmBudget = cagnotte.budgetAnnuel;
          this.cagnotteConfirmAnnee = this.annee;
          this.showCagnotteConfirm = true;
          this.cdr.detectChanges();
        }
      },
      error: () => {
        // Pas de cagnotte pour cette année — l'utilisateur saisit manuellement
      }
    });
  }

  confirmerBudgetCagnotte() {
    this.budgetCagnotte = this.cagnotteConfirmBudget;
    this.showCagnotteConfirm = false;
    this.cdr.detectChanges();
  }

  refuserBudgetCagnotte() {
    this.showCagnotteConfirm = false;
    this.cdr.detectChanges();
  }

  convertirEnMatriciel(lignes: any[]) {
    const prestataireMap = new Map<string, PrestataireRow>();
    const lignesTried = [...lignes].sort((a, b) => (a.ordre ?? 0) - (b.ordre ?? 0));

    lignesTried.forEach(ligne => {
      const key = ligne.prestataireId || ligne.nomFuturPrestataire || 'new';
      if (!prestataireMap.has(key)) {
        prestataireMap.set(key, {
          prestataireId: ligne.prestataireId,
          nomPrestataire: ligne.nomPrestataire || '',
          nomFuturPrestataire: ligne.nomFuturPrestataire,
          useFuturePrestataire: !!ligne.nomFuturPrestataire,
          tjm: ligne.tjm,
          moisData: {},
          totalJours: 0,
          totalRevenu: 0,
          ordre: ligne.ordre ?? prestataireMap.size
        });
      }
      const row = prestataireMap.get(key)!;
      const moisNum = this.extractMoisNumber(ligne.mois);
      row.moisData[moisNum] = { jours: ligne.nombreJours };
      row.totalJours += ligne.nombreJours;
      row.totalRevenu += ligne.revenu;
    });

    this.prestataireRows = Array.from(prestataireMap.values());
  }

  extractMoisNumber(moisStr: string): number {
    const parts = moisStr.split('-');
    return parseInt(parts[1]);
  }

  ajouterPrestataire() {
    this.prestataireRows.push({
      prestataireId: null,
      nomPrestataire: '',
      nomFuturPrestataire: null,
      useFuturePrestataire: false,
      tjm: 0,
      moisData: {},
      totalJours: 0,
      totalRevenu: 0,
      ordre: this.prestataireRows.length
    });
    this.cdr.detectChanges();
  }

  supprimerPrestataire(index: number) {
    this.prestataireRows.splice(index, 1);
    this.reordonner();
    this.calculerBudgetConsomme();
    this.cdr.detectChanges();
  }

  // ✅ Confirmation suppression
  demanderSuppression(index: number): void {
    this.deleteTargetIndex = index;
    this.showDeleteConfirm = true;
  }

  annulerSuppression(): void {
    this.showDeleteConfirm = false;
    this.deleteTargetIndex = null;
  }

  confirmerSuppression(): void {
    if (this.deleteTargetIndex !== null) {
      this.supprimerPrestataire(this.deleteTargetIndex);
    }
    this.showDeleteConfirm = false;
    this.deleteTargetIndex = null;
  }

  // ✅ DRAG & DROP lignes prestataire
  onRowDragStart(event: DragEvent, index: number): void {
    this.draggedIndex = index;
    event.dataTransfer!.effectAllowed = 'move';
    event.dataTransfer!.setData('text/plain', String(index));
  }

  onRowDragOver(event: DragEvent, index: number): void {
    event.preventDefault();
    event.dataTransfer!.dropEffect = 'move';
    this.dragOverIndex = index;
  }

  onRowDragLeave(): void {
    this.dragOverIndex = null;
  }

  onRowDrop(event: DragEvent, toIndex: number): void {
    event.preventDefault();
    this.dragOverIndex = null;
    if (this.draggedIndex === null || this.draggedIndex === toIndex) {
      this.draggedIndex = null;
      return;
    }
    const updated = [...this.prestataireRows];
    const [removed] = updated.splice(this.draggedIndex, 1);
    updated.splice(toIndex, 0, removed);
    updated.forEach((row, idx) => { row.ordre = idx; });
    this.prestataireRows = updated;
    this.draggedIndex = null;
    this.cdr.detectChanges();
  }

  onRowDragEnd(): void {
    this.draggedIndex = null;
    this.dragOverIndex = null;
  }

  reordonner() {
    this.prestataireRows.forEach((row, idx) => { row.ordre = idx; });
  }

  toggleTypePrestataire(row: PrestataireRow) {
    row.useFuturePrestataire = !row.useFuturePrestataire;
    if (row.useFuturePrestataire) {
      row.prestataireId = null;
      row.nomFuturPrestataire = '';
    } else {
      row.nomFuturPrestataire = null;
      row.prestataireId = null;
    }
    this.cdr.detectChanges();
  }

  onPrestataireChange(row: PrestataireRow) {
    if (row.prestataireId) {
      const prestataire = this.prestataires.find(p => p.id === row.prestataireId);
      if (prestataire) {
        row.nomPrestataire = `${prestataire.nom} ${prestataire.prenom}`;
        row.tjm = prestataire.tjm || 0;
        this.calculerTotaux(row);
      }
    }
  }

  onJoursChange(row: PrestataireRow, mois: number, value: string) {
    const jours = parseFloat(value) || 0;
    row.moisData[mois] = { jours };
    this.calculerTotaux(row);
  }

  onTjmChange(row: PrestataireRow) {
    this.calculerTotaux(row);
  }

  calculerTotaux(row: PrestataireRow) {
    row.totalJours = Object.values(row.moisData).reduce((sum, m) => sum + m.jours, 0);
    row.totalRevenu = row.totalJours * row.tjm;
    this.calculerBudgetConsomme();
    this.cdr.detectChanges();
  }

  calculerBudgetConsomme() {
    this.budgetConsomme = this.prestataireRows.reduce((sum, row) => sum + row.totalRevenu, 0);
  }

  getTotalMois(mois: number): number {
    return this.prestataireRows.reduce((sum, row) => sum + (row.moisData[mois]?.jours || 0), 0);
  }

  getTotalRevenuMois(mois: number): number {
    return this.prestataireRows.reduce((sum, row) => {
      const jours = row.moisData[mois]?.jours || 0;
      return sum + (jours * row.tjm);
    }, 0);
  }

  getGrandTotal(): number {
    return this.prestataireRows.reduce((sum, row) => sum + row.totalJours, 0);
  }

  getTotalRevenu(): number {
    return this.prestataireRows.reduce((sum, row) => sum + row.totalRevenu, 0);
  }

  enregistrer() {
    if (!this.annee || this.budgetCagnotte <= 0) {
      this.showNotification('error', 'Veuillez remplir les champs obligatoires');
      return;
    }

    const lignes: any[] = [];
    this.prestataireRows.forEach(row => {
      if (!row.prestataireId && !row.nomFuturPrestataire) return;
      if (row.totalJours === 0) return;

      Object.entries(row.moisData).forEach(([mois, data]) => {
        if (data.jours > 0) {
          lignes.push({
            prestataireId: row.useFuturePrestataire ? null : row.prestataireId,
            nomFuturPrestataire: row.useFuturePrestataire ? row.nomFuturPrestataire : null,
            mois: `${this.annee}-${mois.padStart(2, '0')}`,
            nombreJours: data.jours,
            tjm: row.tjm,
            ordre: row.ordre
          });
        }
      });
    });

    if (lignes.length === 0) {
      this.showNotification('error', 'Ajoutez au moins une ligne avec des jours');
      return;
    }

    if (this.isEditMode && this.estimationId) {
      this.modifier(lignes);
    } else {
      this.creer(lignes);
    }
  }

  creer(lignes: any[]) {
    this.isLoading = true;
    this.loadingMessage = 'Création de l\'estimation en cours...';
    const command: CreateEstimationCommand = {
      annee: this.annee,
      budgetCagnotte: this.budgetCagnotte,
      budgetConsomme: this.budgetConsomme,
      lignes
    };

    this.estimationService.create(command).subscribe({
      next: () => {
        this.cdr.detectChanges();
        this.showNotification('success', 'Estimation créée avec succès !');
        setTimeout(() => this.router.navigate(['/estimations']), 2000);
        this.isLoading = false;
      },
      error: (err) => {
        console.log(err)
        this.cdr.detectChanges();
        this.showNotification('error', err.error?.message || 'Erreur lors de la création');
        this.isLoading = false;
      }
    });
  }

  modifier(lignes: any[]) {
    this.isLoading = true;
    this.loadingMessage = 'Modification de l\'estimation en cours...';
    const command: UpdateEstimationCommand = {
      budgetCagnotte: this.budgetCagnotte,
      budgetConsomme: this.budgetConsomme,
      lignes: lignes.map(l => ({ ...l, id: null }))
    };

    this.estimationService.update(this.estimationId!, command).subscribe({
      next: () => {
        this.cdr.detectChanges();
        this.showNotification('success', 'Estimation modifiée avec succès !');
        setTimeout(() => this.router.navigate(['/estimations']), 2000);
        this.isLoading = false;
      },
      error: (err) => {
        this.cdr.detectChanges();
        this.showNotification('error', err.error?.message || 'Erreur lors de la modification');
        this.isLoading = false;
      }
    });
  }

  annuler() {
    this.router.navigate(['/estimations']);
  }

  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    this.notification = { show: true, type, message };
    setTimeout(() => {
      this.notification.show = false;
      this.cdr.detectChanges();
    }, 5000);
  }
}