import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatMontant',
  standalone: true
})
export class FormatMontantPipe implements PipeTransform {
  transform(montant: number | null | undefined): string {
    if (montant === null || montant === undefined || isNaN(montant)) {
      return '0 €';
    }

    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(montant);
  }
}
