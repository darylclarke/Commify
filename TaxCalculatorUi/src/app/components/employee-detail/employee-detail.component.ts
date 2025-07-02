import { Component, OnInit, signal, computed, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { TaxService } from '../../services/tax.service';
import { Employee } from '../../models/employee.model';
import { TaxCalculationResult } from '../../models/tax-calculation-result.model';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-detail.component.html'
})
export class EmployeeDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private employeeService = inject(EmployeeService);
  private taxService = inject(TaxService);

  employee = signal<Employee | undefined>(undefined);
  taxCalculation = signal<TaxCalculationResult | undefined>(undefined);
  loading = signal(false);
  error = signal('');
  
  showEditDialog = signal(false);
  newSalary = signal(0);
  updating = signal(false);

  // Computed signals for derived values
  employeeName = computed(() => {
    const emp = this.employee();
    return emp ? `${emp.firstName} ${emp.lastName}` : '';
  });

  canSaveSalary = computed(() => {
    const emp = this.employee();
    const salary = this.newSalary();
    return emp && salary > 0;
  });

  isLoading = computed(() => this.loading() || this.updating());

  constructor() {
    // Effect to automatically load tax calculation when employee changes
    effect(() => {
      const emp = this.employee();
      if (emp) {
        this.loadTaxCalculation(emp.salary);
      }
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEmployee(id);
    }
  }

  loadEmployee(id: string): void {
    this.loading.set(true);
    this.error.set('');

    this.employeeService.getEmployee(id).subscribe({
      next: (employee) => {
        this.employee.set(employee);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load employee details. Please try again.');
        this.loading.set(false);
        console.error('Error loading employee:', err);
      }
    });
  }

  loadTaxCalculation(salary: number): void {
    this.taxService.calculateTax(salary).subscribe({
      next: (result) => {
        this.taxCalculation.set(result);
      },
      error: (err) => {
        console.error('Error calculating tax:', err);
      }
    });
  }

  openEditDialog(): void {
    const emp = this.employee();
    this.newSalary.set(emp?.salary || 0);
    this.showEditDialog.set(true);
  }

  cancelEdit(): void {
    this.showEditDialog.set(false);
    this.newSalary.set(0);
  }

  saveSalary(): void {
    if (!this.canSaveSalary()) {
      return;
    }

    const emp = this.employee();
    const salary = this.newSalary();
    
    if (!emp || !salary || salary <= 0) {
      return;
    }

    this.updating.set(true);
    this.employeeService.updateEmployeeSalary(emp.id, salary).subscribe({
      next: (updatedEmployee) => {
        this.employee.set(updatedEmployee);
        this.showEditDialog.set(false);
        this.updating.set(false);
      },
      error: (err) => {
        this.error.set('Failed to update salary. Please try again.');
        this.updating.set(false);
        console.error('Error updating salary:', err);
      }
    });
  }
}
