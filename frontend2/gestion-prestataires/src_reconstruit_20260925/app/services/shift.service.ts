import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
 import { environment } from '../../environments/environment';
import { Shift, TypeShift } from '../models/Shift.model';

@Injectable({
  providedIn: 'root'
})
export class ShiftService {
  private apiUrl = `${environment.apiUrl}/shifts`;
  private typeShiftApiUrl = `${environment.apiUrl}/typeshifts`;

  constructor(private http: HttpClient) {}

  // Shifts
  // ✅ AMÉLIORATION: Support demi-journées (0.5j ou 1j)
  createShift(PrestataireId: string, Date: Date, TypeShiftId: string, Notes?: string, NombreJours?: number): Observable<string> {
    return this.http.post<string>(this.apiUrl, {
        prestataireId: PrestataireId,        // ✅ camelCase pour .NET
        date: Date.toISOString(),            // ✅ camelCase pour .NET
        typeShiftId: TypeShiftId,            // ✅ camelCase pour .NET
        notes: Notes,                        // ✅ camelCase pour .NET
        nombreJours: NombreJours || 1        // ✅ camelCase pour .NET
    });
  }

  updateShift(id: string, typeShiftId: string, notes?: string, NombreJours?: number): Observable<void> {
 
    return this.http.put<void>(`${this.apiUrl}/${id}`, {
      typeShiftId,                           // ✅ déjà camelCase
      notes,                                 // ✅ déjà camelCase
      nombreJours: NombreJours               // ✅ CORRECTION: camelCase pour .NET
    });
  }

  deleteShift(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getShiftsByMonth(year: number, month: number, prestataireId?: string): Observable<Shift[]> {
    let params = new HttpParams();
    if (prestataireId) {
      params = params.set('prestataireId', prestataireId);
    }

    return this.http.get<Shift[]>(`${this.apiUrl}/month/${year}/${month}`, { params });
  }

  getShiftsByWeek(weekStartDate: Date, prestataireId?: string): Observable<Shift[]> {
    let params = new HttpParams()
      .set('weekStartDate', weekStartDate.toISOString());
    
    if (prestataireId) {
      params = params.set('prestataireId', prestataireId);
    }

    return this.http.get<Shift[]>(`${this.apiUrl}/week`, { params });
  }

  getShiftsByDateRange(startDate: Date, endDate: Date, prestataireId?: string | null): Observable<Shift[]> {
    let params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());
    
     // ✅ n'ajoute le paramètre QUE si c'est un vrai Guid string
      const pid = (prestataireId ?? '').trim();
      if (pid !== '' && pid.toLowerCase() !== 'null' && pid.toLowerCase() !== 'undefined') {
        params = params.set('prestataireId', pid);
      }

    return this.http.get<Shift[]>(`${this.apiUrl}/range`, { params });
  }

  // TypeShifts
  getAllTypeShifts(activeOnly: boolean = true): Observable<TypeShift[]> {
    let params = new HttpParams().set('activeOnly', activeOnly.toString());
    return this.http.get<TypeShift[]>(this.typeShiftApiUrl, { params });
  }

  createTypeShift(typeShift: Partial<TypeShift>): Observable<string> {
    return this.http.post<string>(this.typeShiftApiUrl, typeShift);
  }

  updateTypeShift(id: string, typeShift: Partial<TypeShift>): Observable<void> {
    return this.http.put<void>(`${this.typeShiftApiUrl}/${id}`, typeShift);
  }

  deleteTypeShift(id: string): Observable<void> {
    return this.http.delete<void>(`${this.typeShiftApiUrl}/${id}`);
  }
}