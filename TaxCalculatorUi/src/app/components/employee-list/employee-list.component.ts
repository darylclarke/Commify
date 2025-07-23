import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { PagedResult } from '../../models/paged-result.model';
import { EmployeeQueryParameters } from '../../models/employee-query-parameters.model';
import {
  switchMap,
  tap,
  distinctUntilChanged,
  debounceTime,
} from 'rxjs/operators';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './employee-list.component.html',
})
export class EmployeeListComponent implements OnInit {
  private employeeService = inject(EmployeeService);
  private router = inject(Router);

  searchControl = new FormControl('');
  pageSize = 10;

  loading = signal(false);
  employees = signal<Employee[]>([]);
  pagedResult = signal<PagedResult<Employee> | null>(null);
  error = signal<string | null>(null);

  constructor() {}

  ngOnInit(): void {
    // Initial load
    this.load(null, 1).subscribe();

    // Set up debounced search
    this.searchControl.valueChanges
      .pipe(
        debounceTime(700),
        distinctUntilChanged(),
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap((term) => this.load(term, 1))
      )
      .subscribe();
  }

  previousPage(): void {
    const pr = this.pagedResult();
    if (pr?.hasPreviousPage) {
      this.load(this.searchControl.value, pr.pageNumber - 1).subscribe();
    }
  }

  nextPage(): void {
    const pr = this.pagedResult();
    if (pr?.hasNextPage) {
      this.load(this.searchControl.value, pr.pageNumber + 1).subscribe();
    }
  }

  private load(term: string | null, pageNumber: number) {
    const params: EmployeeQueryParameters = {
      pageNumber: pageNumber,
      pageSize: this.pageSize,
      searchTerm: term || undefined,
    };

    return this.employeeService.getEmployees(params).pipe(
      tap({
        next: (result) => {
          this.employees.set(result.items);
          this.pagedResult.set(result);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load employees.');
          this.loading.set(false);
        },
      })
    );
  }

  viewEmployee(id: string): void {
    this.router.navigate(['/employee', id]);
  }
}
