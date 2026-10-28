import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
  })
  export class Commun {
formatMontant(montant: number): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(montant);
  }
}