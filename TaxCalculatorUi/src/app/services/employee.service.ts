import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { EmployeeQueryParameters } from '../models/employee-query-parameters.model';
import { PagedResult } from '../models/paged-result.model';
import { Employee } from '../models/employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getEmployees(params: EmployeeQueryParameters = {}): Observable<PagedResult<Employee>> {
    let httpParams = new HttpParams();
    
    if (params.pageNumber) {
      httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    }
    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }

    return this.http.get<PagedResult<Employee>>(`${this.apiUrl}/v1.0/employees`, { params: httpParams });
  }

  getEmployee(id: string): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/v1.0/employee/${id}`);
  }

  updateEmployeeSalary(id: string, salary: number): Observable<Employee> {
    const params = new HttpParams().set('salary', salary.toString());
    return this.http.put<Employee>(`${this.apiUrl}/v1.0/employee/${id}`, null, { params });
  }
} 