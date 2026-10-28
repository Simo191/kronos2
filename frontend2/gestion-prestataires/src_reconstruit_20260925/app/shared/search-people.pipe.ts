import { Pipe, PipeTransform } from '@angular/core';

//   Filtre une liste de personnes (nom + prénom) selon un terme de recherche.
//   pure: false → réévalué à chaque cycle, la liste pouvant changer sans nouvelle référence.
@Pipe({ name: 'searchPeople', standalone: true, pure: false })
export class SearchPeoplePipe implements PipeTransform {
  transform<T extends { nom: string; prenom: string }>(list: T[], term: string): T[] {
    if (!term?.trim()) return list;
    const q = term.toLowerCase();
    return list.filter(p => (p.nom + ' ' + p.prenom).toLowerCase().includes(q));
  }
}
