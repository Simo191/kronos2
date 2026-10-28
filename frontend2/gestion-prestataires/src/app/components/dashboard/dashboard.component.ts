import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { PermissionService, Permission } from '../../services/permission.service';
import { PrestataireService } from '../../services/prestataire.service';
import { ContratService } from '../../services/contrat.service';
import { CagnotteService } from '../../services/cagnotte.service';
import { JourTravailService, JourTravailDto } from '../../services/jour-travail.service';
import { User } from '../../models/user.model';

import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';
import { environment } from '../../../environments/environment';

interface ContratFin {
  id: string;
  prestataireId: string;
  prestataireNom: string;
  typeContrat: string;
  dateDebut: Date;
  dateFin: Date;
  joursRestants: number;
  joursUtilises: number;
  quotaTotal: number;
  daysUntilExpiry: number;
  isExpire: boolean;       // date dépassée OU quota épuisé
  isQuotaEpuise: boolean;  // jours restants = 0
}

interface PrestataireJours {
  prestataireId: string;
  prestataireNom: string;
  // ⚠️ tjm fixe supprimé du calcul (on garde juste pour affichage “fallback” si besoin)
  tjmFallback?: number;
  mois: { [key: number]: number }; // month => jours
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, FormatMontantPipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  currentUser: User | null = null;

  // 🔹 Toutes les données brutes
  private allJoursTravail: JourTravailDto[] = [];

  // ✅ Tous les contrats (pour récupérer TJM par mois)
  private allContrats: any[] = [];

  // Statistiques générales
  nbrPrestataires = 0;
  nbrContrats = 0;
  cagnotteBudget = 0;
  imageUrl? : string = '' ;
  // Contrats se terminant bientôt
  contratsAVenir: ContratFin[] = [];

  // Année sélectionnée (tableaux N / N+1)
  selectedYear = new Date().getFullYear();

  // Année sélectionnée pour le graphique
  selectedYearForChart: number = new Date().getFullYear();
  selectedPrestataireForChart: string = 'all';

  availableYears: number[] = [];

  prestatairesSelect: {
    id: string; nom: string; prenom: string; imageUrl?: string;
    entreprise?: { id: string; nom: string; code?: string | null } | null;
  }[] = [];

  // ANNÉE N
  prestatairesMoisN: PrestataireJours[] = [];
  filteredPrestatairesMoisN: PrestataireJours[] = [];
  filterPrestataireN = '';
  currentPageN = 1;
  isLoadingN = false;

  // ANNÉE N+1
  prestatairesMoisN1: PrestataireJours[] = [];
  filteredPrestatairesMoisN1: PrestataireJours[] = [];
  filterPrestataireN1 = '';
  currentPageN1 = 1;
  isLoadingN1 = false;

  pageSize = 10;

  moisList = [
    { value: 1, label: 'JANVIER' },
    { value: 2, label: 'FÉVRIER' },
    { value: 3, label: 'MARS' },
    { value: 4, label: 'AVRIL' },
    { value: 5, label: 'MAI' },
    { value: 6, label: 'JUIN' },
    { value: 7, label: 'JUILLET' },
    { value: 8, label: 'AOÛT' },
    { value: 9, label: 'SEPTEMBRE' },
    { value: 10, label: 'OCTOBRE' },
    { value: 11, label: 'NOVEMBRE' },
    { value: 12, label: 'DÉCEMBRE' }
  ];

  isLoading = false;

  // Graphique
  workChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [{
      data: [],
      backgroundColor: '#1e88e5',
      borderRadius: 6,
      maxBarThickness: 40
    }]
  };

  workChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { grid: { display: false } },
      y: { beginAtZero: true, ticks: { precision: 0 } }
    },
    plugins: { legend: { display: false } }
  };

  constructor(
    private authService: AuthService,
    private prestataireService: PrestataireService,
    private contratService: ContratService,
    private cagnotteService: CagnotteService,
    private jourTravailService: JourTravailService,
    private permissionService: PermissionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    this.loadPrestataires();
    this.loadContrats();     // ✅ important (TJM)
    this.loadCagnotte();
    this.loadJoursTravail(); // ✅ montants calculés après
  }

  // ======================================================
  //  Chargement des données
  // ======================================================
  private loadPrestataires(): void {
    this.prestataireService.getAll().subscribe({
      next: (data) => {
        this.nbrPrestataires = data?.filter(p=>p.responsable !== true).length || 0;
        this.prestatairesSelect = data?.filter(p=>p.responsable !== true).map((p: any) => ({
          id: p.id,
          nom: p.nom,
          prenom: p.prenom,
          imageUrl: p.imageUrl ?? null,
          entreprise: p.entreprise ?? null   //   inclus dans la réponse backend
        })) ?? [];
        this.cdr.detectChanges();
      }
    });
  }

  private loadContrats(): void {
    this.contratService.getAll().subscribe({
      next: (data) => {
        this.nbrContrats = data?.length || 0;
        this.allContrats = data || []; // ✅ garder tous les contrats

        // ✅ Recalculer les contrats à venir après avoir chargé les jours travaillés
        this.updateContratsAVenir();

        // ✅ si les jours sont déjà chargés, recalculer la vue (montants)
        if (this.allJoursTravail.length > 0) {
          this.processJoursTravail(this.allJoursTravail);
        }

        this.cdr.detectChanges();
      }
    });
  }

  // ✅ NOUVEAU : Calculer les jours restants correctement
  private updateContratsAVenir(): void {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    this.contratsAVenir = (this.allContrats || [])
      .filter((c: any) => {
        if (!c.dateFin) return false;
        // ✅ Inclure seulement si active=true (même si expiré)
        return (c.active ?? true);
      })
      .map((c: any) => {
        const dateFin = new Date(c.dateFin);

        const quotaTotal = c.nombreJours || c.quotaTotal || 0;
        const joursUtilises = this.calculateJoursUtilises(c.prestataireId, c.dateDebut, c.dateFin);
        const joursRestants = Math.max(0, quotaTotal - joursUtilises);

        const diffMs = dateFin.getTime() - today.getTime();
        const daysUntilExpiry = Math.ceil(diffMs / (1000 * 60 * 60 * 24));

        const isDateExpire = dateFin < today;
        const isQuotaEpuise = joursRestants <= 0;
        const isExpire = isDateExpire || isQuotaEpuise;

        return {
          id: c.id,
          prestataireId: c.prestataireId,
          prestataireNom: `${c.prestataire?.nom ?? ''} ${c.prestataire?.prenom ?? ''}`.trim(),
          typeContrat: c.typeContrat?.nom ?? c.typeContrat?.libelle ?? 'N/A',
          dateDebut: new Date(c.dateDebut),
          dateFin,
          joursRestants,
          joursUtilises,
          quotaTotal,
          daysUntilExpiry,
          isExpire,
          isQuotaEpuise
        };
      })
      // ✅ Inclure contrats : non encore expirés (dateFin future) OU expirés mais active=true
      .filter(c => c.daysUntilExpiry <= 60 || c.isExpire)
      // Trier : non-expirés d'abord par urgence, expirés à la fin
      .sort((a, b) => {
        if (a.isExpire && !b.isExpire) return 1;
        if (!a.isExpire && b.isExpire) return -1;
        return a.daysUntilExpiry - b.daysUntilExpiry;
      })
      .slice(0, 8);
  }

  // ✅ NOUVEAU : Calculer les jours utilisés pour un contrat
  private calculateJoursUtilises(prestataireId: string, dateDebut: any, dateFin: any): number {
    if (!this.allJoursTravail || this.allJoursTravail.length === 0) {
      return 0;
    }

    const debut = new Date(dateDebut);
    const fin = new Date(dateFin);

    return this.allJoursTravail
      .filter(j => j.prestataireId === prestataireId)
      .filter(j => {
        // Chevauchement mois/contrat — évite le bug du jour 15 avant dateDebut
        const [year, month] = j.mois.split('-').map(Number);
        const moisDebut = new Date(year, month - 1, 1);
        const moisFin   = new Date(year, month, 0, 23, 59, 59);
        return moisDebut <= fin && moisFin >= debut;
      })
      .reduce((sum, j) => sum + (j.nombreJours || 0), 0);
  }

  private loadCagnotte(): void {
    // ✅ Ne pas appeler l'API si l'utilisateur n'a pas la permission → évite le 403 dans la console
    if (!this.permissionService.hasPermission(Permission.VIEW_CAGNOTTES)) {
      this.cagnotteBudget = 0;
      return;
    }
    this.cagnotteService.getCagnotteActuelle().subscribe({
      next: (data) => {
        this.cagnotteBudget = data?.budgetRestant ?? 0;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cagnotteBudget = 0;
        this.cdr.detectChanges();
      }
    });
  }

  private loadJoursTravail(): void {
    this.isLoadingN = true;
    this.isLoadingN1 = true;

    this.jourTravailService.getAll().subscribe({
      next: (data) => {
        this.allJoursTravail = data || [];
        this.processJoursTravail(this.allJoursTravail);

        // ✅ IMPORTANT : Recalculer les contrats à venir avec les jours utilisés
        if (this.allContrats.length > 0) {
          this.updateContratsAVenir();
        }

        this.isLoadingN = false;
        this.isLoadingN1 = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingN = false;
        this.isLoadingN1 = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ======================================================
  //  TJM depuis Contrat actif (par mois)
  // ======================================================
  private getTjmFromContrat(prestataireId: string, year: number, month: number): number {
    if (!prestataireId || !year || !month) return 0;
    if (!this.allContrats || this.allContrats.length === 0) return 0;

    const monthStart = new Date(year, month - 1, 1, 0, 0, 0, 0);
    const monthEnd = new Date(year, month, 0, 23, 59, 59, 999);

    const matches = (this.allContrats || [])
      .filter((c: any) => c.prestataireId === prestataireId)
      .filter((c: any) => {
        const start = this.safeDate(c.dateDebut);
        const end = c.dateFin ? this.safeDate(c.dateFin) : null;
        if (!start) return false;

        // actif sur le mois
        const cond1 = start.getTime() <= monthEnd.getTime();
        const cond2 = !end || end.getTime() >= monthStart.getTime();
        return cond1 && cond2;
      })
      // si plusieurs, prendre le plus récent
      .sort((a: any, b: any) => {
        const da = this.safeDate(a.dateDebut)?.getTime() ?? 0;
        const db = this.safeDate(b.dateDebut)?.getTime() ?? 0;
        return db - da;
      });

    const contrat = matches[0];
    if (!contrat) return 0;

    // support des 2 noms possibles
    return Number(contrat.tjm ?? contrat.tauxJournalier ?? 0) || 0;
  }

  private safeDate(v: any): Date | null {
    if (!v) return null;
    const d = new Date(v);
    return isNaN(d.getTime()) ? null : d;
  }

  // ======================================================
  //  Traitement des jours de travail
  // ======================================================
  private processJoursTravail(jours: JourTravailDto[]): void {
    const yearsSet = new Set<number>();
    const prestataireMapN = new Map<string, PrestataireJours>();
    const prestataireMapN1 = new Map<string, PrestataireJours>();

    jours.forEach((jour) => {
      const [year, month] = jour.mois.split('-');
      const y = +year;
      const m = +month;

      yearsSet.add(y);

      // Année N
      if (y === this.selectedYear) {
        if (!prestataireMapN.has(jour.prestataireId)) {
          prestataireMapN.set(jour.prestataireId, {
            prestataireId: jour.prestataireId,
            prestataireNom: `${jour.prestataire?.nom ?? ''} ${jour.prestataire?.prenom ?? ''}`,
            tjmFallback: jour.tjm, // fallback (au cas où pas de contrat)
            mois: {}
          });
        }
        prestataireMapN.get(jour.prestataireId)!.mois[m] =
          (prestataireMapN.get(jour.prestataireId)!.mois[m] || 0) + jour.nombreJours;
      }

      // Année N+1
      if (y === this.selectedYear + 1) {
        if (!prestataireMapN1.has(jour.prestataireId)) {
          prestataireMapN1.set(jour.prestataireId, {
            prestataireId: jour.prestataireId,
            prestataireNom: `${jour.prestataire?.nom ?? ''} ${jour.prestataire?.prenom ?? ''}`,
            tjmFallback: jour.tjm,
            mois: {}
          });
        }
        prestataireMapN1.get(jour.prestataireId)!.mois[m] =
          (prestataireMapN1.get(jour.prestataireId)!.mois[m] || 0) + jour.nombreJours;
      }
    });

    this.availableYears = Array.from(yearsSet).sort((a, b) => b - a);
    if (this.availableYears.length === 0) {
      this.availableYears = [this.selectedYear];
    }

    this.prestatairesMoisN = Array.from(prestataireMapN.values());
    this.filteredPrestatairesMoisN = [...this.prestatairesMoisN];

    this.prestatairesMoisN1 = Array.from(prestataireMapN1.values());
    this.filteredPrestatairesMoisN1 = [...this.prestatairesMoisN1];

    this.currentPageN = 1;
    this.currentPageN1 = 1;

    if (!this.availableYears.includes(this.selectedYearForChart)) {
      this.selectedYearForChart = this.selectedYear;
    }

    this.updateWorkChart();
  }

  changeYear(year: number | string): void {
    this.selectedYear = Number(year);
    this.processJoursTravail(this.allJoursTravail);
  }

  // ======================================================
  //  Filtres
  // ======================================================
  applyFiltersN(): void {
    this.filteredPrestatairesMoisN = this.prestatairesMoisN.filter(p => {
      return !this.filterPrestataireN || p.prestataireId === this.filterPrestataireN;
    });
    this.currentPageN = 1;
  }

  applyFiltersN1(): void {
    this.filteredPrestatairesMoisN1 = this.prestatairesMoisN1.filter(p => {
      return !this.filterPrestataireN1 || p.prestataireId === this.filterPrestataireN1;
    });
    this.currentPageN1 = 1;
  }

  // ======================================================
  //  Calculs
  // ======================================================
  getJoursForMonth(p: PrestataireJours, month: number): number {
    return p.mois[month] || 0;
  }

  // ✅ TJM du contrat actif sur le mois (année N)
  getTjmForMonthN(p: PrestataireJours, month: number): number {
    const tjm = this.getTjmFromContrat(p.prestataireId, this.selectedYear, month);
    return tjm || (p.tjmFallback ?? 0);
  }

  // ✅ TJM du contrat actif sur le mois (année N+1)
  getTjmForMonthN1(p: PrestataireJours, month: number): number {
    const tjm = this.getTjmFromContrat(p.prestataireId, this.selectedYear + 1, month);
    return tjm || (p.tjmFallback ?? 0);
  }

  // ✅ Montant = jours * TJM (contrat du mois)
  getMontantForMonthN(p: PrestataireJours, month: number): number {
    return this.getJoursForMonth(p, month) * this.getTjmForMonthN(p, month);
  }

  getMontantForMonthN1(p: PrestataireJours, month: number): number {
    return this.getJoursForMonth(p, month) * this.getTjmForMonthN1(p, month);
  }

  getTotalJours(p: PrestataireJours): number {
    return this.moisList.reduce((s, m) => s + this.getJoursForMonth(p, m.value), 0);
  }

  // ✅ Total montant = somme des montants mensuels (pas tjm fixe)
  getTotalMontantN(p: PrestataireJours): number {
    return this.moisList.reduce((s, m) => s + this.getMontantForMonthN(p, m.value), 0);
  }

  getTotalMontantN1(p: PrestataireJours): number {
    return this.moisList.reduce((s, m) => s + this.getMontantForMonthN1(p, m.value), 0);
  }

  getTotalGeneralJoursN(): number {
    return this.filteredPrestatairesMoisN.reduce((s, p) => s + this.getTotalJours(p), 0);
  }

  getTotalGeneralMontantN(): number {
    return this.filteredPrestatairesMoisN.reduce((s, p) => s + this.getTotalMontantN(p), 0);
  }

  getTotalGeneralJoursN1(): number {
    return this.filteredPrestatairesMoisN1.reduce((s, p) => s + this.getTotalJours(p), 0);
  }

  getTotalGeneralMontantN1(): number {
    return this.filteredPrestatairesMoisN1.reduce((s, p) => s + this.getTotalMontantN1(p), 0);
  }

  // ======================================================
  //  Pagination N
  // ======================================================
  get totalPagesN(): number {
    return Math.ceil(this.filteredPrestatairesMoisN.length / this.pageSize);
  }

  get pagedPrestataireN(): PrestataireJours[] {
    const start = (this.currentPageN - 1) * this.pageSize;
    return this.filteredPrestatairesMoisN.slice(start, start + this.pageSize);
  }

  get pageNumbersN(): number[] {
    return Array.from({ length: this.totalPagesN }, (_, i) => i + 1);
  }

  get startItemIndexN(): number {
    return this.filteredPrestatairesMoisN.length === 0 ? 0 : (this.currentPageN - 1) * this.pageSize + 1;
  }

  get endItemIndexN(): number {
    return Math.min(this.currentPageN * this.pageSize, this.filteredPrestatairesMoisN.length);
  }

  goToPageN(page: number): void {
    if (page >= 1 && page <= this.totalPagesN) this.currentPageN = page;
  }

  prevPageN(): void {
    if (this.currentPageN > 1) this.currentPageN--;
  }

  nextPageN(): void {
    if (this.currentPageN < this.totalPagesN) this.currentPageN++;
  }

  // ======================================================
  //  Pagination N+1
  // ======================================================
  get totalPagesN1(): number {
    return Math.ceil(this.filteredPrestatairesMoisN1.length / this.pageSize);
  }

  get pagedPrestataireN1(): PrestataireJours[] {
    const start = (this.currentPageN1 - 1) * this.pageSize;
    return this.filteredPrestatairesMoisN1.slice(start, start + this.pageSize);
  }

  get pageNumbersN1(): number[] {
    return Array.from({ length: this.totalPagesN1 }, (_, i) => i + 1);
  }

  get startItemIndexN1(): number {
    return this.filteredPrestatairesMoisN1.length === 0 ? 0 : (this.currentPageN1 - 1) * this.pageSize + 1;
  }

  get endItemIndexN1(): number {
    return Math.min(this.currentPageN1 * this.pageSize, this.filteredPrestatairesMoisN1.length);
  }

  goToPageN1(page: number): void {
    if (page >= 1 && page <= this.totalPagesN1) this.currentPageN1 = page;
  }

  prevPageN1(): void {
    if (this.currentPageN1 > 1) this.currentPageN1--;
  }

  nextPageN1(): void {
    if (this.currentPageN1 < this.totalPagesN1) this.currentPageN1++;
  }

  // ======================================================
  //  Graphique (jours seulement)
  // ======================================================
  updateWorkChart(): void {
    if (!this.allJoursTravail || this.allJoursTravail.length === 0) return;

    const labels: string[] = [];
    const values: number[] = [];
    const targetYear = Number(this.selectedYearForChart);

    const prestataireMap = new Map<string, PrestataireJours>();

    this.allJoursTravail.forEach((jour) => {
      const [year, month] = jour.mois.split('-');
      const y = +year;
      const m = +month;

      if (y === targetYear) {
        if (!prestataireMap.has(jour.prestataireId)) {
          prestataireMap.set(jour.prestataireId, {
            prestataireId: jour.prestataireId,
            prestataireNom: `${jour.prestataire?.nom ?? ''} ${jour.prestataire?.prenom ?? ''}`,
            tjmFallback: jour.tjm,
            mois: {}
          });
        }
        prestataireMap.get(jour.prestataireId)!.mois[m] =
          (prestataireMap.get(jour.prestataireId)!.mois[m] || 0) + jour.nombreJours;
      }
    });

    const dataSource = Array.from(prestataireMap.values());

    this.moisList.forEach((m) => {
      labels.push(m.label.substring(0, 4));

      let sum = 0;
      dataSource.forEach((p) => {
        if (this.selectedPrestataireForChart !== 'all' && this.selectedPrestataireForChart !== p.prestataireId) {
          return;
        }
        sum += (p.mois[m.value] || 0);
      });

      values.push(sum);
    });

    this.workChartData = {
      labels: [...labels],
      datasets: [{
        data: [...values],
        label: 'Jours travaillés',
        backgroundColor: '#1e88e5',
        borderRadius: 6,
        maxBarThickness: 40
      }]
    };

    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  // Divers
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('fr-FR');
  }

  getJoursRestantsClass(jours: number): string {
    if (jours <= 7) return 'bg-danger text-white';
    if (jours <= 30) return 'bg-warning text-dark';
    return 'bg-success text-white';
  }

  getProgressPercent(contrat: ContratFin): number {
    if (!contrat.quotaTotal) return 0;
    return Math.min(Math.round((contrat.joursUtilises / contrat.quotaTotal) * 100), 100);
  }

  getProgressBarClass(contrat: ContratFin): string {
    const pct = this.getProgressPercent(contrat);
    if (pct >= 90) return 'bg-danger';
    if (pct >= 70) return 'bg-warning';
    return 'bg-success';
  }

  getDaysUntilExpiryClass(days: number): string {
    if (days <= 7)  return 'badge bg-danger text-white';
    if (days <= 30) return 'badge bg-warning text-dark';
    return 'badge bg-info text-white';
  }

  getTotalJoursForMonthN(month: number): number {
    return this.filteredPrestatairesMoisN
      .reduce((sum, p) => sum + this.getJoursForMonth(p, month), 0);
  }
  
  getTotalJoursForMonthN1(month: number): number {
    return this.filteredPrestatairesMoisN1
      .reduce((sum, p) => sum + this.getJoursForMonth(p, month), 0);
  }
  

  // ── Avatars prestataires ──────────────────────────────────────────────────
  getInitialesDashboard(nom: string, prenom: string): string {
    return ((prenom?.[0] ?? '') + (nom?.[0] ?? '')).toUpperCase();
  }

  getAvatarUrlDashboard(prestataireId: string): string | null {
     
    this.imageUrl = '';
    const p = this.prestatairesSelect.find(x => x.id === prestataireId);
     this.imageUrl = p?.imageUrl;

    if (!p?.imageUrl) return null;
if (p?.imageUrl.startsWith('http')) return this.imageUrl!;
    return `${environment.apiUrl.replace('/api', '')}/${this.imageUrl}`;
  }

  getInitialesFromNomComplet(nomComplet: string): string {
    const parts = nomComplet.trim().split(' ').filter(Boolean);
    if (parts.length >= 2) return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    return (parts[0]?.[0] ?? '?').toUpperCase();
  }

  getAvatarUrlByNom(prestataireNom: string): string | null {
    const p = this.prestatairesSelect.find(x =>
      `${x.nom} ${x.prenom}`.trim().toLowerCase() === prestataireNom.trim().toLowerCase() ||
      `${x.prenom} ${x.nom}`.trim().toLowerCase() === prestataireNom.trim().toLowerCase()
    );
    return p ? this.getAvatarUrlDashboard(p.id) : null;
  }

}