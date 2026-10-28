import { Component, OnInit, OnDestroy, HostListener, ChangeDetectorRef, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { NotificationCenterService } from '../../services/notification-center.service';
import {
  NotificationItem,
  isExpired,
  isCredential,
  isContrat,
  isContratDate,
  isContratQuota
} from '../../models/notification-center.model';

/**
 *   Cloche de notifications "façon Facebook" :
 *   - Icône bell dans le header
 *   - Badge rouge avec compteur d'éléments à traiter
 *   - Au clic : dropdown avec liste paginée (orange = expire bientôt, rouge = expiré)
 *   - Clic sur un item : navigue vers la page Prestataires sur la fiche concernée
 */
@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-bell.component.html',
  styleUrls: ['./notification-bell.component.scss']
})
export class NotificationBellComponent implements OnInit, OnDestroy {
  notifications: NotificationItem[] = [];
  isOpen = false;
  isRefreshing = false;

  private sub?: Subscription;

  //   Exposés au template via fonctions utilitaires (pas de pipes / pas de *ngIf)
  isExpired = isExpired;
  isCredential = isCredential;
  isContrat = isContrat;
  isContratDate = isContratDate;
  isContratQuota = isContratQuota;

  constructor(
    private notifCenter: NotificationCenterService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private elRef: ElementRef
  ) {}

  ngOnInit(): void {
    this.sub = this.notifCenter.notifications$.subscribe(list => {
      this.notifications = list;
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  //   Total = nombre de notifs (servira pour le badge)
  get totalCount(): number {
    return this.notifications.length;
  }

  //   Compteur des seules notifications "rouges" (déjà expirées) pour le badge
  get expiredCount(): number {
    return this.notifications.filter(n => isExpired(n)).length;
  }

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) this.forceRefresh();
  }

  closeDropdown(): void {
    this.isOpen = false;
  }

  forceRefresh(): void {
    this.isRefreshing = true;
    this.notifCenter.refresh().subscribe({
      next: () => {
        this.isRefreshing = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isRefreshing = false;
        this.cdr.detectChanges();
      }
    });
  }

  //   Ferme le dropdown quand on clique hors du composant
  @HostListener('document:click', ['$event.target'])
  onClickOutside(target: EventTarget | null): void {
    if (!this.isOpen) return;
    const host = this.elRef.nativeElement as HTMLElement;
    if (host && target instanceof Node && !host.contains(target)) {
      this.isOpen = false;
    }
  }

  /**
   *   Au clic sur une notification, on redirige vers le module concerné.
   *   - Contrat → page Contrats filtrée sur ce prestataire
   *   - Credential → page Prestataires (le user ouvrira ses comptes lui-même)
   */
  openNotification(n: NotificationItem): void {
    this.isOpen = false;
    if (isContrat(n)) {
      this.router.navigate(['/contrats'], { queryParams: { prestataireId: n.prestataireId } });
    } else {
      this.router.navigate(['/prestataires']);
    }
  }

  //   CSS class par sévérité pour styler les items.
  itemClass(n: NotificationItem): string {
    return isExpired(n) ? 'notif-item notif-danger' : 'notif-item notif-warning';
  }
}
