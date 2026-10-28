import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, timer } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { NotificationItem } from '../models/notification-center.model';

/**
 *   Centre de notifications "façon Facebook" — distinct du `NotificationService`
 *   (toast). Charge les expirations contrats + comptes ADM depuis le backend,
 *   diffuse en BehaviorSubject pour que la cloche dans le header s'auto-rafraîchit.
 */
@Injectable({ providedIn: 'root' })
export class NotificationCenterService {
  private apiUrl = `${environment.apiUrl}/notifications`;

  //   Refresh auto toutes les 5 minutes ; les utilisateurs peuvent rafraîchir
  //   manuellement au clic sur la cloche.
  private readonly POLL_INTERVAL_MS = 5 * 60 * 1000;

  private items$ = new BehaviorSubject<NotificationItem[]>([]);
  public notifications$: Observable<NotificationItem[]> = this.items$.asObservable();

  constructor(private http: HttpClient) {}

  /**
   *   Démarre le polling. À appeler une fois après login (ex : depuis AppComponent
   *   quand `authService.currentUserValue` devient non-null).
   */
  startAutoRefresh(): void {
    timer(0, this.POLL_INTERVAL_MS)
      .pipe(switchMap(() => this.refresh()))
      .subscribe();
  }

  /** Force un rafraîchissement immédiat (ex : au clic sur la cloche). */
  refresh(): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(this.apiUrl).pipe(
      tap(list => this.items$.next(list ?? [])),
      catchError(() => {
        //   En cas d'erreur (offline, 401…), on garde l'ancien état et on retourne
        //   un tableau vide pour ne pas casser la pipeline.
        return of(this.items$.value);
      })
    );
  }

  /** Compteur d'items pour le badge de la cloche. */
  get currentCount(): number {
    return this.items$.value.length;
  }

  /** Reset (logout). */
  clear(): void {
    this.items$.next([]);
  }
}
