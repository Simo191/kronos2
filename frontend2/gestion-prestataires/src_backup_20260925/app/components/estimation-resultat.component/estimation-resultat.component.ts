import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EstimationService } from '../../services/estimation.service';
import { Estimation } from '../../models/estimation.model';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';

interface PrestataireRow {
  prestataireId: string | null;
  nom: string;
  moisData: Map<string, { jours: number; tjm: number; revenu: number }>;
  totalJours: number;
  totalRevenu: number;
}

@Component({
  selector: 'app-estimation-resultat',
  standalone: true,
  imports: [CommonModule, FormsModule, FormatMontantPipe],
  templateUrl: './estimation-resultat.component.html',
  styleUrls: ['./estimation-resultat.component.scss']
})
export class EstimationResultatComponent implements OnInit {
  estimationId!: string;
  estimation?: Estimation;
  isLoading = false;

  // Tableau dashboard
  prestataireRows: PrestataireRow[] = [];
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

  constructor(
    private estimationService: EstimationService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.estimationId = id;
        this.loadEstimation(id);
      }
    });
  }

  loadEstimation(id: string) {
    this.isLoading = true;
    this.estimationService.getById(id).subscribe({
      next: (data) => {
        this.estimation = data;
        this.buildTableau();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Erreur chargement:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  buildTableau() {
    if (!this.estimation) return;

    // Grouper par prestataire
    const prestataireMap = new Map<string, PrestataireRow>();

    this.estimation.lignes.forEach(ligne => {
      // Clé unique : prestataireId OU nomFuturPrestataire
      const key = ligne.prestataireId || ligne.nomFuturPrestataire || 'inconnu';
      const nom = ligne.nomPrestataire || ligne.nomFuturPrestataire || 'Inconnu';

      if (!prestataireMap.has(key)) {
        prestataireMap.set(key, {
          prestataireId: ligne.prestataireId!,
          nom: nom,
          moisData: new Map(),
          totalJours: 0,
          totalRevenu: 0
        });
      }

      const row = prestataireMap.get(key)!;
      
      // Extraire le mois du format "2025-01"
      const moisNum = this.extractMoisFromString(ligne.mois);
      const moisKey = moisNum.toString();

      row.moisData.set(moisKey, {
        jours: ligne.nombreJours,
        tjm: ligne.tjm,
        revenu: ligne.revenu
      });

      row.totalJours += ligne.nombreJours;
      row.totalRevenu += ligne.revenu;
    });

    this.prestataireRows = Array.from(prestataireMap.values());
  }

  extractMoisFromString(moisStr: string): number {
    // Format attendu: "2025-01" → retourne 1
    const parts = moisStr.split('-');
    return parts.length === 2 ? parseInt(parts[1]) : 0;
  }

  getMoisKey(moisValue: number): string {
    return moisValue.toString();
  }

  getDataForMois(row: PrestataireRow, moisValue: number): { jours: number; tjm: number; revenu: number } | null {
    const moisKey = this.getMoisKey(moisValue);
    return row.moisData.get(moisKey) || null;
  }

  getTotalJoursForMois(moisValue: number): number {
    let total = 0;
    this.prestataireRows.forEach(row => {
      const data = this.getDataForMois(row, moisValue);
      if (data) {
        total += data.jours;
      }
    });
    return total;
  }

  getTotalRevenuForMois(moisValue: number): number {
    let total = 0;
    this.prestataireRows.forEach(row => {
      const data = this.getDataForMois(row, moisValue);
      if (data) {
        total += data.revenu;
      }
    });
    return total;
  }

  getGrandTotalJours(): number {
    return this.prestataireRows.reduce((sum, row) => sum + row.totalJours, 0);
  }

  getGrandTotalRevenu(): number {
    return this.prestataireRows.reduce((sum, row) => sum + row.totalRevenu, 0);
  }

  retourListe() {
    this.router.navigate(['/estimations']);
  }

  editer() {
    this.router.navigate(['/estimations', this.estimationId, 'detail']);
  }

  getBudgetRestant(): number {
    if (!this.estimation) return 0;
    return this.estimation.budgetCagnotte - this.estimation.budgetConsomme;
  }

  // ─── Helpers ───────────────────────────────────────────────────────────────

  private fmt(n: number): string {
    return n.toLocaleString('fr-FR') + ' €';
  }

  private fmtJ(j: number): string {
    return j.toLocaleString('fr-FR', { maximumFractionDigits: 1 }) + 'j';
  }

  private buildExportHtml(forPrint = false): string {
    const e = this.estimation!;
    const budgetRestant = this.getBudgetRestant();
    const grandTotalJ = this.getGrandTotalJours();
    const grandTotalR = this.getGrandTotalRevenu();
    const ecart = e.budgetCagnotte - grandTotalR;

    // Stat cards
    const cards = [
      { label: 'BUDGET CAGNOTTE',  val: this.fmt(e.budgetCagnotte),          color: '#1e88e5' },
      { label: 'BUDGET CONSOMMÉ',  val: this.fmt(e.budgetConsomme),           color: '#e53935' },
      { label: 'BUDGET RESTANT',   val: this.fmt(budgetRestant),              color: budgetRestant >= 0 ? '#43a047' : '#e53935' },
      { label: 'TOTAL ESTIMÉ',     val: this.fmt(grandTotalR),                color: '#00acc1' }
    ];

    const cardsHtml = cards.map(c => `
      <div class="stat-card">
        <div class="stat-val" style="color:${c.color}">${c.val}</div>
        <div class="stat-label">${c.label}</div>
      </div>`).join('');

    // Table header
    const thMois = this.moisList.map(m => `
      <th>
        ${m.label}<br>
        <small>(${this.fmtJ(this.getTotalJoursForMois(m.value))})</small>
      </th>`).join('');

    // Table rows
    const rows = this.prestataireRows.map(row => {
      const tdMois = this.moisList.map(mois => {
        const d = this.getDataForMois(row, mois.value);
        if (!d) return '<td class="empty">-</td>';
        return `<td class="has-data">
          <span class="badge-j">${this.fmtJ(d.jours)}</span><br>
          <span class="amount">${d.revenu.toLocaleString('fr-FR')} €</span><br>
          <span class="tjm">TJM: ${d.tjm.toLocaleString('fr-FR')} €</span>
        </td>`;
      }).join('');

      return `<tr>
        <td class="col-presta">
          <strong>${row.nom}</strong><br>
          <small>${row.prestataireId ? 'Existant' : 'Futur'}</small>
        </td>
        ${tdMois}
        <td class="col-total">
          <strong>${this.fmtJ(row.totalJours)}</strong><br>
          <small>${this.fmt(row.totalRevenu)}</small>
        </td>
      </tr>`;
    }).join('');

    // Totaux row
    const tdTotaux = this.moisList.map(mois => `
      <td class="col-total">
        <strong>${this.fmtJ(this.getTotalJoursForMois(mois.value))}</strong><br>
        <small>${this.fmt(this.getTotalRevenuForMois(mois.value))}</small>
      </td>`).join('');

    const printBtn = forPrint ? '' : '';
    const pageBreak = forPrint ? 'page-break-inside: avoid;' : '';

    return `<!DOCTYPE html>
<html lang="fr">
<head>
  <meta charset="UTF-8">
  <title>Estimation ${e.annee}</title>
  <style>
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body { font-family: Arial, sans-serif; font-size: 13px; color: #333; background: #f5f5f5; padding: 24px; }
    h1  { font-size: 22px; color: #1e88e5; margin-bottom: 4px; }
    h2  { font-size: 15px; color: #555; font-weight: normal; margin-bottom: 20px; }
    h3  { font-size: 14px; color: #333; margin: 20px 0 10px; }

    /* Stat cards */
    .cards { display: flex; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
    .stat-card { flex: 1; min-width: 180px; background: #fff; border-radius: 8px; padding: 16px 20px;
                 box-shadow: 0 1px 4px rgba(0,0,0,.12); }
    .stat-val   { font-size: 20px; font-weight: 700; }
    .stat-label { font-size: 10px; color: #888; text-transform: uppercase; margin-top: 4px; }

    /* Table */
    .tbl-wrap { background: #fff; border-radius: 8px; overflow: auto; box-shadow: 0 1px 4px rgba(0,0,0,.12); }
    .tbl-head  { background: #1e88e5; color: #fff; padding: 12px 16px; font-size: 14px; font-weight: 600; }
    table      { border-collapse: collapse; width: 100%; font-size: 11px; }
    th, td     { border: 1px solid #ddd; padding: 6px 8px; text-align: center; vertical-align: middle; }
    thead tr:first-child th { background: #1e88e5; color: #fff; text-align: center; font-size: 13px; }
    thead tr:last-child th  { background: #e3f2fd; color: #1e88e5; font-size: 10px; text-transform: uppercase; }
    .col-presta { text-align: left !important; min-width: 130px; background: #fafafa; font-size: 11px; }
    .col-total  { background: #e8f5e9; font-size: 11px; }
    .has-data   { background: #f0f7ff; }
    .empty      { color: #bbb; }
    .badge-j    { background: #1e88e5; color: #fff; border-radius: 3px; padding: 1px 5px; font-size: 10px; font-weight: 700; }
    .amount     { color: #333; font-weight: 600; font-size: 11px; }
    .tjm        { color: #888; font-size: 10px; }
    .row-total  { background: #e0e0e0 !important; font-weight: 700; }
    .row-total td { background: #e0e0e0; }
    .row-total td.col-total { background: #1e88e5 !important; color: #fff !important; }
    .row-total td.col-total small { color: rgba(255,255,255,.85) !important; }

    /* Récap */
    .recap { display: flex; gap: 12px; padding: 16px; background: #f9f9f9; border-top: 1px solid #eee; flex-wrap: wrap; }
    .recap-box { flex: 1; min-width: 160px; border-radius: 6px; padding: 10px 14px; font-size: 12px; }
    .recap-box.info    { background: #e3f2fd; border-left: 4px solid #1e88e5; }
    .recap-box.success { background: #e8f5e9; border-left: 4px solid #43a047; }
    .recap-box.warning { background: #fff8e1; border-left: 4px solid #fb8c00; }
    .recap-box .r-label { font-weight: 600; }
    .recap-box .r-val   { font-size: 18px; font-weight: 700; float: right; }

    /* Print */
    @media print {
      body  { background: #fff; padding: 8px; }
      .no-print { display: none !important; }
      .tbl-wrap { box-shadow: none; overflow: visible; }
      table { font-size: 9px; }
      th, td { padding: 4px 5px; }
      .stat-val { font-size: 16px; }
    }
  </style>
</head>
<body>
  <h1><span style="font-size:18px">📊</span> Résultat Estimation ${e.annee}</h1>
  <h2>Vue tableau de bord des jours et revenus par prestataire et par mois</h2>

  <div class="cards">${cardsHtml}</div>

  <div class="tbl-wrap">
    <div class="tbl-head">📅 Tableau des jours et revenus par mois — ${e.annee}</div>
    <table>
      <thead>
        <tr><th colspan="${2 + this.moisList.length}">ANNÉE ${e.annee}</th></tr>
        <tr>
          <th class="col-presta">PRESTATAIRE</th>
          ${thMois}
          <th class="col-total">TOTAL<br><small>(${this.fmtJ(grandTotalJ)})</small></th>
        </tr>
      </thead>
      <tbody>
        ${rows}
        <tr class="row-total">
          <td class="col-presta"><strong>TOTAL GÉNÉRAL</strong></td>
          ${tdTotaux}
          <td class="col-total" style="background:#1e88e5;color:#fff">
            <strong style="font-size:13px">${this.fmtJ(grandTotalJ)}</strong><br>
            <small style="color:rgba(255,255,255,.85)">${this.fmt(grandTotalR)}</small>
          </td>
        </tr>
      </tbody>
    </table>

    <div class="recap">
      <div class="recap-box info">
        <span class="r-label">📅 Total jours</span>
        <span class="r-val">${this.fmtJ(grandTotalJ)}</span>
      </div>
      <div class="recap-box success">
        <span class="r-label">💰 Total estimé</span>
        <span class="r-val">${this.fmt(grandTotalR)}</span>
      </div>
      <div class="recap-box ${ecart < 0 ? 'warning' : 'success'}">
        <span class="r-label">⚖️ Écart budget</span>
        <span class="r-val">${this.fmt(ecart)}</span>
      </div>
    </div>
  </div>

  <p style="margin-top:12px;font-size:10px;color:#aaa;text-align:right">
    Généré le ${new Date().toLocaleDateString('fr-FR')}
  </p>
</body>
</html>`;
  }

  exportExcel() {
    if (!this.estimation) return;

    const html = this.buildExportHtml(false);
    const blob = new Blob([html], { type: 'application/vnd.ms-excel;charset=utf-8;' });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = `Estimation_${this.estimation.annee}.xls`;
    a.click();
    URL.revokeObjectURL(url);
  }

  exportPDF() {
    if (!this.estimation) return;

    const html = this.buildExportHtml(true);
    const win  = window.open('', '_blank', 'width=1200,height=900');
    if (!win) return;
    win.document.write(html);
    win.document.close();
    win.focus();
    setTimeout(() => {
      win.print();
    }, 600);
  }

  showNotification(type: 'success' | 'error', message: string) {
    // Notification simple (à améliorer avec un vrai service de notification)
   }
}