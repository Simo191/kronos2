import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface AppNotification {
  id: number;
  type: NotificationType;
  title: string;
  message: string;
  duration: number;      // ms — 0 = permanent
  createdAt: number;     // timestamp pour la barre de progression
}

let _idCounter = 0;

const TYPE_TITLES: Record<NotificationType, string> = {
  success: 'Succès',
  error:   'Erreur',
  warning: 'Attention',
  info:    'Information'
};

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private _notifications$ = new BehaviorSubject<AppNotification[]>([]);
  readonly notifications$ = this._notifications$.asObservable();

  // ─── API publique ───────────────────────────────────────────────────────────

  success(message: string, title?: string, duration = 4500) {
    this._add('success', message, title ?? TYPE_TITLES.success, duration);
  }

  error(message: string, title?: string, duration = 6000) {
    this._add('error', message, title ?? TYPE_TITLES.error, duration);
  }

  warning(message: string, title?: string, duration = 5000) {
    this._add('warning', message, title ?? TYPE_TITLES.warning, duration);
  }

  info(message: string, title?: string, duration = 4500) {
    this._add('info', message, title ?? TYPE_TITLES.info, duration);
  }

  // Compatibilité avec l'ancien showNotification(type, message)
  show(type: NotificationType, message: string, duration?: number) {
    this._add(type, message, TYPE_TITLES[type], duration);
  }

  dismiss(id: number) {
    this._notifications$.next(
      this._notifications$.getValue().filter(n => n.id !== id)
    );
  }

  clear() {
    this._notifications$.next([]);
  }

  // ─── Interne ─────────────────────────────────────────────────────────────────

  private _add(type: NotificationType, message: string, title: string, duration?: number) {
    const durations: Record<NotificationType, number> = {
      success: 4500, error: 6000, warning: 5000, info: 4500
    };
    const d = duration ?? durations[type];
    const notif: AppNotification = {
      id: _idCounter++,
      type,
      title,
      message: message.replace(/^[✅❌⚠️ℹ️]+\s*/, ''), // Nettoyer les emojis en préfixe
      duration: d,
      createdAt: Date.now()
    };

    this._notifications$.next([...this._notifications$.getValue(), notif]);

    if (d > 0) {
      setTimeout(() => this.dismiss(notif.id), d);
    }
  }
}