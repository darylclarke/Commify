import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TaxCalculationResult } from '../models/tax-calculation-result.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TaxService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  calculateTax(salary: number): Observable<TaxCalculationResult> {
    return this.http.get<TaxCalculationResult>(`${this.apiUrl}/v1.0/tax/calculate/${salary}`);
  }
} 