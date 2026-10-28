import { Pipe, PipeTransform } from '@angular/core';
import { Prestataire } from '../models/prestataire.model';

@Pipe({ name: 'searchPeople', standalone: true, pure: false })
export class SearchPeoplePipe implements PipeTransform {
  transform(people: Prestataire[], search: string): Prestataire[] {
    if (!search?.trim()) return people;
    const q = search.toLowerCase();
    return people.filter(p =>
      (p.nom + ' ' + p.prenom).toLowerCase().includes(q)
    );
  }
}