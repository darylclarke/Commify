import { Component, signal, computed, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { PagedResult } from '../../models/paged-result.model';
import { EmployeeQueryParameters } from '../../models/employee-query-parameters.model';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-list.component.html',
})
export class EmployeeListComponent {
  private employeeService = inject(EmployeeService);
  private router = inject(Router);

  employees = signal<Employee[]>([]);
  pagedResult = signal<PagedResult<Employee> | undefined>(undefined);
  loading = signal(false);
  error = signal('');

  searchTerm = signal('');
  currentPage = signal(1);
  pageSize = signal(5);

  constructor() {
    effect(() => {
      // Load employees when currentPage, pageSize, or searchTerm changes
      this.loadEmployees();
    });
  }

  loadEmployees(): void {
    this.loading.set(true);
    this.error.set('');

    const params: EmployeeQueryParameters = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm() || undefined
    };

    this.employeeService.getEmployees(params).subscribe({
      next: (result) => {
        this.pagedResult.set(result);
        this.employees.set(result.items);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load employees. Please try again.');
        this.loading.set(false);
        console.error('Error loading employees:', err);
      }
    });
  }

  onSearch(): void {
    this.currentPage.set(1);
    this.loadEmployees();
  }

  previousPage(): void {
    if (this.pagedResult()?.hasPreviousPage) {
      this.currentPage.update(page => page - 1);
      this.loadEmployees();
    }
  }

  nextPage(): void {
    if (this.pagedResult()?.hasNextPage) {
      this.currentPage.update(page => page + 1);
      this.loadEmployees();
    }
  }

  viewEmployee(id: string): void {
    this.router.navigate(['/employee', id]);
  }
} 
