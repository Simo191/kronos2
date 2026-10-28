import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ContratService,
  SuiviApplication,
  SuiviCommandeLigne,
  SuiviCommandes,
  TrancheContrat
} from '../../services/contrat.service';
import { FormatMontantPipe } from '../../shared/format-montant-pipe';
import { NotificationService } from '../../services/notification.service';
import { environment } from '../../../environments/environment';

//  Un bloc du tableau = un logiciel (application du planning) et ses lignes de commande
interface GroupeLogiciel {
  cle: string;
  nom: string;
  couleur: string;
  statut: string;
  lignes: SuiviCommandeLigne[];
  montantTotal: number;
  montantRestant: number;
  nbLignesAffichees: number;   // lignes + commentaires (rowspan de l'export Excel)
}

interface TotalAnnee {
  annee: number;
  jours: number;
  montant: number;
}

interface LigneTotal {
  libelle: string;
  classe: string;
  annees: TotalAnnee[];
  jours: number;
  montantTotal: number;
  joursConsommes: number;
  montantConsomme: number;
  joursRestants: number;
  montantRestant: number;
}

@Component({
  selector: 'app-suivi-commandes',
  standalone: true,
  imports: [CommonModule, FormsModule, FormatMontantPipe],
  templateUrl: './suivi-commandes.component.html',
  styleUrls: ['./suivi-commandes.component.scss']
})
export class SuiviCommandesComponent implements OnInit {
  data: SuiviCommandes = { applications: [], lignes: [] };
  isLoading = false;

  // Filtres
  searchTerm = '';
  filterEntreprise = '';
  filterApplication = '';
  filterTranche: '' | TrancheContrat = '';
  masquerTerminees = false;
  afficherLogicielsVides = true;
  afficherConsommation = true;

  // Affichage (comme l'écran Collaborateurs)
  viewMode: 'list' | 'cards' = 'cards';
  groupesReplies = new Set<string>();

  // Vue calculée
  annees: number[] = [];
  groupes: GroupeLogiciel[] = [];
  logicielsVides: SuiviApplication[] = [];
  totaux: LigneTotal[] = [];
  entreprises: string[] = [];

  // Choix du logiciel directement depuis le tableau
  editionLogicielId: string | null = null;
  enregistrementLogiciel = false;

  constructor(
    private contratService: ContratService,
    private notifService: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.contratService.getSuiviCommandes().subscribe({
      next: (data) => {
        this.data = data ?? { applications: [], lignes: [] };
        this.entreprises = [...new Set(this.data.lignes
          .map(l => this.getEntrepriseLabel(l))
          .filter(e => !!e))].sort((a, b) => a.localeCompare(b, 'fr'));
        this.isLoading = false;
        this.recalculer();
      },
      error: () => {
        this.isLoading = false;
        this.notifService.show('error', 'Erreur lors du chargement du suivi des commandes');
        this.cdr.detectChanges();
      }
    });
  }

  // ========== FILTRES + REGROUPEMENT ==========

  recalculer(): void {
    const lignes = this.data.lignes.filter(l => this.ligneVisible(l));

    // Années affichées : toutes celles qui ont du prévu ou du consommé sur les lignes visibles
    this.annees = [...new Set(lignes.flatMap(l => l.annees.map(a => a.annee)))].sort((a, b) => a - b);

    const applications = this.applicationsParOrdre;
    const groupes: GroupeLogiciel[] = [];
    for (const app of applications) {
      if (this.filterApplication && this.filterApplication !== app.id) continue;
      const lignesApp = lignes.filter(l => l.applicationId === app.id);
      if (lignesApp.length) groupes.push(this.creerGroupe(app.id, app.nom, app.couleur, app.statut, lignesApp));
    }

    if (!this.filterApplication || this.filterApplication === 'aucun') {
      const sansLogiciel = lignes.filter(l => !l.applicationId || !applications.some(a => a.id === l.applicationId));
      if (sansLogiciel.length) {
        groupes.push(this.creerGroupe('aucun', 'Sans logiciel', '#94a3b8', '', sansLogiciel));
      }
    }

    // Logiciels du planning sans aucune commande (listés à part, en bas)
    const avecCommande = new Set(this.data.lignes.map(l => l.applicationId).filter(Boolean));
    this.logicielsVides = applications.filter(a => !avecCommande.has(a.id));

    this.groupes = groupes;
    this.totaux = this.calculerTotaux(groupes.flatMap(g => g.lignes));
    this.cdr.detectChanges();
  }

  private ligneVisible(l: SuiviCommandeLigne): boolean {
    const q = this.searchTerm.trim().toLowerCase();
    if (q) {
      const texte = [l.prestataireNom, l.numeroCommande, l.referenceCtr, l.applicationNom, this.getEntrepriseLabel(l)]
        .filter(Boolean).join(' ').toLowerCase();
      if (!texte.includes(q)) return false;
    }
    if (this.filterEntreprise && this.getEntrepriseLabel(l) !== this.filterEntreprise) return false;
    if (this.filterTranche && l.tranche !== this.filterTranche) return false;
    if (this.masquerTerminees && this.estTerminee(l)) return false;
    return true;
  }

  private creerGroupe(cle: string, nom: string, couleur: string, statut: string, lignes: SuiviCommandeLigne[]): GroupeLogiciel {
    const triees = [...lignes].sort((a, b) =>
      a.prestataireNom.localeCompare(b.prestataireNom, 'fr')
      || (a.tranche === b.tranche ? 0 : a.tranche === 'Ferme' ? -1 : 1)
      || new Date(a.dateDebut).getTime() - new Date(b.dateDebut).getTime());
    return {
      cle,
      nom,
      couleur: couleur || '#3b82f6',
      statut,
      lignes: triees,
      montantTotal: triees.reduce((s, l) => s + l.montantTotal, 0),
      montantRestant: triees.reduce((s, l) => s + l.montantRestant, 0),
      nbLignesAffichees: Math.max(1, triees.length + triees.filter(l => !!l.commentaire).length)
    };
  }

  //  Totaux séparés ferme / option, comme dans le fichier Excel
  private calculerTotaux(lignes: SuiviCommandeLigne[]): LigneTotal[] {
    const total = (libelle: string, classe: string, source: SuiviCommandeLigne[]): LigneTotal => ({
      libelle,
      classe,
      annees: this.annees.map(annee => ({
        annee,
        jours: source.reduce((s, l) => s + this.getAnnee(l, annee).joursPrevus, 0),
        montant: source.reduce((s, l) => s + this.getAnnee(l, annee).montantPrevu, 0)
      })),
      jours: source.reduce((s, l) => s + l.nombreJours, 0),
      montantTotal: source.reduce((s, l) => s + l.montantTotal, 0),
      joursConsommes: source.reduce((s, l) => s + l.joursConsommes, 0),
      montantConsomme: source.reduce((s, l) => s + l.montantConsomme, 0),
      joursRestants: source.reduce((s, l) => s + l.joursRestants, 0),
      montantRestant: source.reduce((s, l) => s + l.montantRestant, 0)
    });

    return [
      total('Total ferme', 'total-ferme', lignes.filter(l => l.tranche !== 'Option')),
      total('Total option', 'total-option', lignes.filter(l => l.tranche === 'Option')),
      total('Total général', 'total-general', lignes)
    ];
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.filterEntreprise = '';
    this.filterApplication = '';
    this.filterTranche = '';
    this.masquerTerminees = false;
    this.recalculer();
  }

  toggleViewMode(mode: 'list' | 'cards'): void {
    this.viewMode = mode;
  }

  toggleGroupe(cle: string): void {
    if (this.groupesReplies.has(cle)) this.groupesReplies.delete(cle);
    else this.groupesReplies.add(cle);
  }

  estReplie(cle: string): boolean {
    return this.groupesReplies.has(cle);
  }

  // ========== LOGICIEL : CONFIRMER / CHANGER DEPUIS LE TABLEAU ==========

  //  Logiciel déduit du planning alors que le prestataire est sur plusieurs logiciels
  logicielAConfirmer(l: SuiviCommandeLigne): boolean {
    return l.applicationSource === 'planning' && l.logicielsPlanning.length > 1;
  }

  get nbLogicielsAConfirmer(): number {
    return this.data.lignes.filter(l => this.logicielAConfirmer(l)).length;
  }

  ouvrirChoixLogiciel(l: SuiviCommandeLigne): void {
    this.editionLogicielId = this.editionLogicielId === l.contratId ? null : l.contratId;
  }

  autresLogiciels(l: SuiviCommandeLigne): SuiviApplication[] {
    const affectes = new Set(l.logicielsPlanning.map(x => x.id));
    return this.applicationsParOrdre.filter(a => !affectes.has(a.id));
  }

  //  valeur '' = revenir au logiciel du planning (applicationId null)
  choisirLogiciel(l: SuiviCommandeLigne, applicationId: string): void {
    this.enregistrementLogiciel = true;
    this.contratService.updateApplication(l.contratId, applicationId || null).subscribe({
      next: () => {
        this.enregistrementLogiciel = false;
        this.editionLogicielId = null;
        this.notifService.show('success', 'Logiciel enregistré pour cette commande');
        this.load();
      },
      error: () => {
        this.enregistrementLogiciel = false;
        this.notifService.show('error', "Erreur lors de l'enregistrement du logiciel");
        this.cdr.detectChanges();
      }
    });
  }

  // ========== AFFICHAGE ==========

  get applicationsParOrdre(): SuiviApplication[] {
    return [...this.data.applications]
      .sort((a, b) => (a.ordre - b.ordre) || a.nom.localeCompare(b.nom, 'fr'));
  }

  get applicationsTriees(): SuiviApplication[] {
    return [...this.data.applications].sort((a, b) => a.nom.localeCompare(b.nom, 'fr'));
  }

  getAnnee(l: SuiviCommandeLigne, annee: number) {
    return l.annees.find(a => a.annee === annee)
      ?? { annee, joursPrevus: 0, montantPrevu: 0, joursConsommes: 0, montantConsomme: 0 };
  }

  getEntrepriseLabel(l: SuiviCommandeLigne): string {
    return l.entrepriseCode || l.entrepriseNom || '';
  }

  estTerminee(l: SuiviCommandeLigne): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return new Date(l.dateFin) < today;
  }

  pourcentageConsomme(l: { montantConsomme: number; montantTotal: number }): number {
    if (!l.montantTotal) return 0;
    return Math.round((l.montantConsomme / l.montantTotal) * 100);
  }

  classeConsommation(l: { montantConsomme: number; montantTotal: number }): string {
    const p = this.pourcentageConsomme(l);
    if (p >= 100) return 'bg-danger';
    if (p >= 85) return 'bg-warning';
    return 'bg-success';
  }

  getAvatarUrl(l: SuiviCommandeLigne): string | null {
    const url = l.prestataireImageUrl;
    if (!url) return null;
    return url.startsWith('http') ? url : `${environment.apiUrl.replace('/api', '')}/${url}`;
  }

  getInitiales(nom: string): string {
    const parts = (nom || '').trim().split(/\s+/).filter(Boolean);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase() || '?';
  }

  //  Format court du fichier Excel : "févr-26"
  moisCourt(date: string): string {
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    const mois = d.toLocaleDateString('fr-FR', { month: 'short' }).replace('.', '');
    return `${mois}-${String(d.getFullYear()).slice(-2)}`;
  }

  infoAnnee(l: SuiviCommandeLigne, annee: number): string {
    const a = this.getAnnee(l, annee);
    return `${annee} — prévu ${this.formatJours(a.joursPrevus)} j, consommé ${this.formatJours(a.joursConsommes)} j (${this.formatEuros(a.montantConsomme)})`;
  }

  formatJours(j: number): string {
    return new Intl.NumberFormat('fr-FR', { maximumFractionDigits: 1 }).format(j || 0);
  }

  formatEuros(m: number): string {
    return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 }).format(m || 0);
  }

  //  Colonnes de la vue liste : Prestataire, Commande, TJM/Jours + années + Total (+ Consommé, Reste)
  get nbColonnesEcran(): number {
    return 3 + this.annees.length + 1 + (this.afficherConsommation ? 2 : 0);
  }

  get nbLignes(): number {
    return this.groupes.reduce((s, g) => s + g.lignes.length, 0);
  }

  get totalGeneral(): LigneTotal | undefined {
    return this.totaux.find(t => t.classe === 'total-general');
  }

  get totalFerme(): LigneTotal | undefined {
    return this.totaux.find(t => t.classe === 'total-ferme');
  }

  get totalOption(): LigneTotal | undefined {
    return this.totaux.find(t => t.classe === 'total-option');
  }

  //  Vue cartes : années où la ligne a du prévu
  anneesPrevues(l: SuiviCommandeLigne) {
    return l.annees.filter(a => a.joursPrevus > 0 || a.montantPrevu > 0);
  }

  //  Vue cartes : récapitulatif ferme / option / total par année
  get recapAnnees(): { annee: number; ferme: number; option: number; total: number; jours: number }[] {
    return this.annees.map((annee, i) => ({
      annee,
      ferme: this.totalFerme?.annees[i]?.montant ?? 0,
      option: this.totalOption?.annees[i]?.montant ?? 0,
      total: this.totalGeneral?.annees[i]?.montant ?? 0,
      jours: this.totalGeneral?.annees[i]?.jours ?? 0
    }));
  }

  // ========== EXPORT EXCEL (HTML .xls, présentation du fichier d'origine) ==========

  exportExcel(): void {
    const nbColonnes = 1 + 8 + this.annees.length * 2 + 1 + (this.afficherConsommation ? 4 : 0);
    const esc = (v: any) => String(v ?? '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    const th = (v: string, extra = '') =>
      `<td style="background:#1E293B;color:white;font-weight:bold;text-align:center;border:1px solid #374151;padding:5px" ${extra}>${v}</td>`;
    const td = (v: any, style = '', extra = '') =>
      `<td style="border:1px solid #CBD5E1;padding:4px 6px;${style}" ${extra}>${esc(v)}</td>`;
    const num = (v: number) => (Math.round((v || 0) * 100) / 100).toString().replace('.', ',');

    const enTete1 = [
      th('Logiciel', 'rowspan="2"'), th('Commande', 'rowspan="2"'), th('Société', 'rowspan="2"'), th('CTR', 'rowspan="2"'),
      th('Prestataire', 'rowspan="2"'), th('TJM', 'rowspan="2"'), th('Jours', 'rowspan="2"'), th('Début', 'rowspan="2"'),
      th('Fin', 'rowspan="2"'),
      ...this.annees.map(a => th(String(a), 'colspan="2"')),
      th('Total', 'rowspan="2"'),
      ...(this.afficherConsommation ? [th('Consommé', 'colspan="2"'), th('Reste', 'colspan="2"')] : [])
    ].join('');
    const enTete2 = [
      ...this.annees.flatMap(() => [th('j'), th('€')]),
      ...(this.afficherConsommation ? [th('j'), th('€'), th('j'), th('€')] : [])
    ].join('');

    let corps = '';
    for (const g of this.groupes) {
      g.lignes.forEach((l, i) => {
        const couleur = l.tranche === 'Option' ? 'color:#DC2626;' : '';
        const cellules = [
          i === 0 ? td(g.nom, 'font-weight:bold;background:#F1F5F9;vertical-align:middle', `rowspan="${g.nbLignesAffichees}"`) : '',
          td(`${l.numeroCommande ? 'Commande n° ' + l.numeroCommande : 'Commande'} ${l.tranche === 'Option' ? 'option' : 'ferme'}`, couleur),
          td(this.getEntrepriseLabel(l), couleur),
          td(l.referenceCtr ?? '', couleur),
          td(l.prestataireNom, couleur),
          td(num(l.tjm), couleur + 'text-align:right'),
          td(num(l.nombreJours), couleur + 'text-align:right'),
          td(this.moisCourt(l.dateDebut), couleur + 'text-align:center'),
          td(this.moisCourt(l.dateFin), couleur + 'text-align:center'),
          ...this.annees.flatMap(a => {
            const v = this.getAnnee(l, a);
            return [
              td(v.joursPrevus ? num(v.joursPrevus) : '', couleur + 'text-align:right;font-style:italic'),
              td(v.montantPrevu ? num(v.montantPrevu) : '', couleur + 'text-align:right')
            ];
          }),
          td(num(l.montantTotal), couleur + 'text-align:right;font-weight:bold'),
          ...(this.afficherConsommation ? [
            td(num(l.joursConsommes), 'text-align:right'), td(num(l.montantConsomme), 'text-align:right'),
            td(num(l.joursRestants), 'text-align:right'), td(num(l.montantRestant), 'text-align:right')
          ] : [])
        ];
        corps += `<tr>${cellules.join('')}</tr>`;
        if (l.commentaire) {
          corps += `<tr>${td(l.commentaire, 'color:#DC2626;font-size:10px', `colspan="${nbColonnes - 1}"`)}</tr>`;
        }
      });
    }
    // Logiciels sans commande en fin de tableau (comme BDDE / SEIMS dans le fichier d'origine)
    for (const app of this.filterApplication ? [] : this.logicielsVides) {
      corps += `<tr>${td(app.nom, 'font-weight:bold;background:#F1F5F9')}${td('', '', `colspan="${nbColonnes - 1}"`)}</tr>`;
    }

    const pied = this.totaux.map(t => `<tr>${[
      td(t.libelle, 'font-weight:bold;background:#E2E8F0', 'colspan="9"'),
      ...t.annees.flatMap(a => [
        td(a.jours ? num(a.jours) : '', 'text-align:right;background:#E2E8F0'),
        td(a.montant ? num(a.montant) : '', 'text-align:right;font-weight:bold;background:#E2E8F0')
      ]),
      td(num(t.montantTotal), 'text-align:right;font-weight:bold;background:#E2E8F0'),
      ...(this.afficherConsommation ? [
        td(num(t.joursConsommes), 'text-align:right;background:#E2E8F0'), td(num(t.montantConsomme), 'text-align:right;background:#E2E8F0'),
        td(num(t.joursRestants), 'text-align:right;background:#E2E8F0'), td(num(t.montantRestant), 'text-align:right;background:#E2E8F0')
      ] : [])
    ].join('')}</tr>`).join('');

    const xls = `<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel">
<head><meta charset="UTF-8"></head><body>
<table style="border-collapse:collapse;font-family:Calibri,Arial;font-size:11px">
  <tr>${enTete1}</tr>
  <tr>${enTete2}</tr>
  ${corps}
  ${pied}
</table>
</body></html>`;

    const today = new Date().toLocaleDateString('fr-FR').replace(/\//g, '-');
    const blob = new Blob(['﻿' + xls], { type: 'application/vnd.ms-excel;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Suivi_commandes_${today}.xls`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }
}
