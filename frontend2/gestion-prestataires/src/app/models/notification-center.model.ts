/**
 *   Notification d'expiration calculée côté serveur.
 *   Trois catégories distinctes (un même contrat peut générer 2 notifs
 *   indépendantes : une "date" + une "quota").
 */
export type NotificationKind =
  | 'ContratDate' | 'ContratQuota' | 'Credential'
  | 0 | 1 | 2;
export type NotificationSeverity = 'Warning' | 'Danger' | 0 | 1;

export interface NotificationItem {
  id: string;
  kind: NotificationKind;
  severity: NotificationSeverity;
  prestataireId: string;
  prestataireName: string;
  title: string;
  subtitle: string;
  expiresAt: string;
  daysUntilExpiry: number;
}

//   Helpers de normalisation (le backend peut serialize l'enum en string ou en int
//   selon la config JSON ; on gère les deux pour rester robuste).
export function isExpired(n: NotificationItem): boolean {
  return n.severity === 'Danger' || n.severity === 1 || n.daysUntilExpiry < 0;
}
export function isContratDate(n: NotificationItem): boolean {
  return n.kind === 'ContratDate' || n.kind === 0;
}
export function isContratQuota(n: NotificationItem): boolean {
  return n.kind === 'ContratQuota' || n.kind === 1;
}
export function isCredential(n: NotificationItem): boolean {
  return n.kind === 'Credential' || n.kind === 2;
}
//   Vrai si la notif concerne un contrat (l'une ou l'autre catégorie).
export function isContrat(n: NotificationItem): boolean {
  return isContratDate(n) || isContratQuota(n);
}
