import {
    Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
  } from '@angular/core';
  import { CommonModule } from '@angular/common';
  import { Subscription } from 'rxjs';
import { AppNotification, NotificationService } from '../../services/notification.service';
   
  @Component({
    selector: 'app-notification-toast',
    standalone: true,
    imports: [CommonModule],
    changeDetection: ChangeDetectionStrategy.OnPush,
    template: `
      <div class="toast-stack" aria-live="polite" aria-atomic="false">
        @for (n of notifications; track n.id) {
          <div
            class="toast-item toast-{{ n.type }}"
            [class.toast-exit]="exiting.has(n.id)"
            role="alert">
  
            <!-- Icône -->
            <div class="toast-icon-wrap">
              <div class="toast-icon-ring"></div>
              <i class="bi {{ icons[n.type] }} toast-icon"></i>
            </div>
  
            <!-- Contenu -->
            <div class="toast-body">
              <div class="toast-title">{{ n.title }}</div>
              <div class="toast-message">{{ n.message }}</div>
            </div>
  
            <!-- Fermer -->
            <button class="toast-close" (click)="close(n.id)" aria-label="Fermer">
              <i class="bi bi-x"></i>
            </button>
  
            <!-- Barre de progression -->
            @if (n.duration > 0) {
              <div class="toast-progress">
                <div
                  class="toast-progress-bar"
                  [style.animation-duration.ms]="n.duration">
                </div>
              </div>
            }
          </div>
        }
      </div>
    `,
    styles: [`
      /* ─── Conteneur ─────────────────────────────────────────────────────── */
      .toast-stack {
        position: fixed;
        top: 24px;
        right: 24px;
        z-index: 99999;
        display: flex;
        flex-direction: column;
        gap: 12px;
        max-width: 400px;
        pointer-events: none;
      }
  
      /* ─── Toast de base ──────────────────────────────────────────────────── */
      .toast-item {
        position: relative;
        display: flex;
        align-items: flex-start;
        gap: 14px;
        padding: 16px 16px 20px;          /* 20px bas pour la barre */
        border-radius: 14px;
        min-width: 320px;
        max-width: 400px;
        pointer-events: all;
        overflow: hidden;
        backdrop-filter: blur(16px);
        -webkit-backdrop-filter: blur(16px);
        box-shadow:
          0 4px 6px -1px rgba(0,0,0,.1),
          0 10px 30px -5px rgba(0,0,0,.18),
          inset 0 1px 0 rgba(255,255,255,.15);
  
        /* Entrée */
        animation: toastIn .35s cubic-bezier(.21,1.02,.73,1) both;
      }
  
      .toast-exit {
        animation: toastOut .25s ease-in forwards !important;
      }
  
      @keyframes toastIn {
        from { opacity: 0; transform: translateX(110%) scale(.92); }
        to   { opacity: 1; transform: translateX(0) scale(1); }
      }
      @keyframes toastOut {
        to   { opacity: 0; transform: translateX(110%) scale(.88); }
      }
  
      /* ─── Couleurs par type ─────────────────────────────────────────────── */
      .toast-success {
        background: linear-gradient(135deg,
          rgba(16,185,129,.92) 0%,
          rgba(5,150,105,.95) 100%);
        color: #fff;
        border: 1px solid rgba(52,211,153,.4);
      }
      .toast-error {
        background: linear-gradient(135deg,
          rgba(239,68,68,.93) 0%,
          rgba(185,28,28,.96) 100%);
        color: #fff;
        border: 1px solid rgba(252,165,165,.35);
      }
      .toast-warning {
        background: linear-gradient(135deg,
          rgba(245,158,11,.93) 0%,
          rgba(180,83,9,.96) 100%);
        color: #fff;
        border: 1px solid rgba(253,230,138,.35);
      }
      .toast-info {
        background: linear-gradient(135deg,
          rgba(59,130,246,.93) 0%,
          rgba(29,78,216,.96) 100%);
        color: #fff;
        border: 1px solid rgba(147,197,253,.35);
      }
  
      /* ─── Icône animée ──────────────────────────────────────────────────── */
      .toast-icon-wrap {
        position: relative;
        flex-shrink: 0;
        width: 38px;
        height: 38px;
        display: flex;
        align-items: center;
        justify-content: center;
      }
  
      .toast-icon-ring {
        position: absolute;
        inset: 0;
        border-radius: 50%;
        background: rgba(255,255,255,.18);
        animation: ringPop .4s .1s cubic-bezier(.175,.885,.32,1.275) both;
      }
  
      @keyframes ringPop {
        from { transform: scale(0); opacity: 0; }
        to   { transform: scale(1); opacity: 1; }
      }
  
      .toast-icon {
        position: relative;
        font-size: 18px;
        filter: drop-shadow(0 1px 2px rgba(0,0,0,.2));
        animation: iconBounce .5s .15s cubic-bezier(.175,.885,.32,1.275) both;
      }
  
      @keyframes iconBounce {
        from { transform: scale(0) rotate(-15deg); opacity: 0; }
        to   { transform: scale(1) rotate(0deg);   opacity: 1; }
      }
  
      /* ─── Corps ─────────────────────────────────────────────────────────── */
      .toast-body {
        flex: 1;
        min-width: 0;
      }
  
      .toast-title {
        font-size: 13px;
        font-weight: 700;
        letter-spacing: .3px;
        text-transform: uppercase;
        opacity: .75;
        margin-bottom: 3px;
      }
  
      .toast-message {
        font-size: 14px;
        font-weight: 500;
        line-height: 1.45;
        word-break: break-word;
      }
  
      /* ─── Bouton fermer ─────────────────────────────────────────────────── */
      .toast-close {
        flex-shrink: 0;
        width: 26px;
        height: 26px;
        border: none;
        background: rgba(255,255,255,.18);
        border-radius: 50%;
        color: #fff;
        font-size: 14px;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: background .2s, transform .15s;
        margin-top: -2px;
      }
      .toast-close:hover {
        background: rgba(255,255,255,.32);
        transform: scale(1.1);
      }
  
      /* ─── Barre de progression ──────────────────────────────────────────── */
      .toast-progress {
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        height: 4px;
        background: rgba(255,255,255,.18);
        border-radius: 0 0 14px 14px;
        overflow: hidden;
      }
  
      .toast-progress-bar {
        height: 100%;
        width: 100%;
        background: rgba(255,255,255,.7);
        border-radius: 0 0 14px 14px;
        transform-origin: left;
        animation: progressShrink linear both;
      }
  
      @keyframes progressShrink {
        from { transform: scaleX(1); }
        to   { transform: scaleX(0); }
      }
  
      /* ─── Responsive ─────────────────────────────────────────────────────── */
      @media (max-width: 480px) {
        .toast-stack {
          top: 12px;
          right: 12px;
          left: 12px;
          max-width: none;
        }
        .toast-item {
          min-width: 0;
          max-width: none;
        }
      }
    `]
  })
  export class NotificationToastComponent implements OnInit, OnDestroy {
    notifications: AppNotification[] = [];
    exiting = new Set<number>();
  
    readonly icons: Record<string, string> = {
      success: 'bi-check-circle-fill',
      error:   'bi-x-circle-fill',
      warning: 'bi-exclamation-triangle-fill',
      info:    'bi-info-circle-fill'
    };
  
    private sub!: Subscription;
  
    constructor(
      private notifService: NotificationService,
      private cdr: ChangeDetectorRef
    ) {}
  
    ngOnInit() {
      this.sub = this.notifService.notifications$.subscribe(list => {
        this.notifications = list;
        this.cdr.markForCheck();
      });
    }
  
    ngOnDestroy() { this.sub?.unsubscribe(); }
  
    close(id: number) {
      this.exiting.add(id);
      this.cdr.markForCheck();
      setTimeout(() => {
        this.exiting.delete(id);
        this.notifService.dismiss(id);
      }, 260);
    }
  }