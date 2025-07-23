import { CommonModule } from '@angular/common';
import { Component, inject, input, model, output, signal } from '@angular/core';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-update-salary-dialog',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './update-salary-dialog.component.html',
  styleUrl: './update-salary-dialog.component.css',
})
export class UpdateSalaryDialogComponent {
  employeeService = inject(EmployeeService);

  currentSalary = input(0);
  showEdit = model(false);

  newSalary = new FormControl(0, {
    validators: [
      Validators.required,
      Validators.min(1),
      Validators.pattern('^[0-9]'),
    ],
  });

  updating = signal(false);

  employeeReturned = model<Employee | null>();

  constructor(private toastr: ToastrService) {}

  cancelEdit(): void {
    this.showEdit.update(() => false);
  }

  saveSalary(): void {
    if (
      !this.employeeReturned()?.id ||
      !this.newSalary.value ||
      this.updating()
    ) {
      return;
    }

    const salary = this.newSalary.value;

    this.updating.set(true);
    this.employeeService
      .updateEmployeeSalary(this.employeeReturned()?.id!, salary)
      .subscribe({
        next: (updatedEmployee) => {
          this.employeeReturned.update(() => updatedEmployee);
          this.showEdit.update(() => false);
          this.updating.set(false);
        },
        error: (err) => {
          this.toastr.error('Failed to update salary. Please try again.');
          this.updating.set(false);
          console.error('Error updating salary:', err);
        },
      });
  }
}
