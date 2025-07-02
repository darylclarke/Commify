import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { EmployeeDetailComponent } from './employee-detail.component';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { TaxService } from '../../services/tax.service';
import { Employee } from '../../models/employee.model';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { TaxCalculationResult } from '../../models/tax-calculation-result.model';

describe('EmployeeDetailComponent', () => {
  let component: EmployeeDetailComponent;
  let fixture: ComponentFixture<EmployeeDetailComponent>;
  let mockEmployeeService: jasmine.SpyObj<EmployeeService>;
  let mockTaxService: jasmine.SpyObj<TaxService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: any;

  const mockEmployee: Employee = {
    id: '123',
    firstName: 'John',
    lastName: 'Doe',
    salary: 50000,
    createdAt: '2024-01-01T00:00:00Z'
  };

  const mockTaxCalculation: TaxCalculationResult = {
    grossAnnualSalary: 50000,
    grossMonthlySalary: 4166.67,
    netAnnualSalary: 40000,
    netMonthlySalary: 3333.33,
    annualTaxPaid: 10000,
    monthlyTaxPaid: 833.33
  };

  beforeEach(async () => {
    const employeeServiceSpy = jasmine.createSpyObj('EmployeeService', ['getEmployee', 'updateEmployeeSalary']);
    const taxServiceSpy = jasmine.createSpyObj('TaxService', ['calculateTax']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    mockActivatedRoute = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('123')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [EmployeeDetailComponent],
      providers: [
        { provide: EmployeeService, useValue: employeeServiceSpy },
        { provide: TaxService, useValue: taxServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeDetailComponent);
    component = fixture.componentInstance;
    mockEmployeeService = TestBed.inject(EmployeeService) as jasmine.SpyObj<EmployeeService>;
    mockTaxService = TestBed.inject(TaxService) as jasmine.SpyObj<TaxService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load employee details on init', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();

    expect(mockEmployeeService.getEmployee).toHaveBeenCalledWith('123');
    expect(mockTaxService.calculateTax).toHaveBeenCalledWith(50000);
    expect(component.employee()).toEqual(mockEmployee);
    expect(component.taxCalculation()).toEqual(mockTaxCalculation);
    expect(component.loading()).toBeFalse();
  }));

  it('should display employee information when loaded', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const employeeName = fixture.debugElement.query(By.css('.display-5'));
    expect(employeeName.nativeElement.textContent.trim()).toBe('John Doe');
  }));

  it('should display tax calculation details', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const grossAnnualSalary = fixture.debugElement.query(By.css('.list-group-item:nth-child(1) span:last-child'));
    expect(grossAnnualSalary.nativeElement.textContent.trim()).toContain('£50,000.00');
  }));

  it('should handle error when loading employee fails', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(throwError(() => new Error('Network error')));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    expect(component.error()).toBe('Failed to load employee details. Please try again.');
    
    const errorElement = fixture.debugElement.query(By.css('.alert-danger'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent.trim()).toContain('Failed to load employee details');
  }));

  it('should open edit dialog when edit icon is clicked', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const editIcon = fixture.debugElement.query(By.css('img[alt="Edit Salary"]'));
    editIcon.nativeElement.click();
    fixture.detectChanges();

    expect(component.showEditDialog()).toBeTrue();
    expect(component.newSalary()).toBe(50000);
    
    const modal = fixture.debugElement.query(By.css('.modal'));
    expect(modal).toBeTruthy();
  }));

  it('should close edit dialog when cancel is clicked', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    // Open dialog
    component.openEditDialog();
    fixture.detectChanges();

    // Click cancel
    const cancelButton = fixture.debugElement.query(By.css('.btn-secondary'));
    cancelButton.nativeElement.click();
    fixture.detectChanges();

    expect(component.showEditDialog()).toBeFalse();
    expect(component.newSalary()).toBe(0);
  }));

  it('should update salary when save is clicked', fakeAsync(() => {
    const updatedEmployee = { ...mockEmployee, salary: 60000 };
    const updatedTaxCalculation = { ...mockTaxCalculation, grossAnnualSalary: 60000 };

    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));
    mockEmployeeService.updateEmployeeSalary.and.returnValue(of(updatedEmployee));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    // Open dialog and set new salary
    component.openEditDialog();
    component.newSalary.set(60000);
    fixture.detectChanges();

    // Click save
    const saveButton = fixture.debugElement.query(By.css('.btn-primary'));
    saveButton.nativeElement.click();
    tick();
    fixture.detectChanges();

    expect(mockEmployeeService.updateEmployeeSalary).toHaveBeenCalledWith('123', 60000);
    expect(component.employee()).toEqual(updatedEmployee);
    expect(component.showEditDialog()).toBeFalse();
  }));

  it('should disable save button when salary is invalid', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    // Open dialog with invalid salary
    component.openEditDialog();
    component.newSalary.set(0);
    fixture.detectChanges();

    const saveButton = fixture.debugElement.query(By.css('.btn-primary'));
    expect(saveButton.nativeElement.disabled).toBeTrue();
  }));

  it('should show loading state when updating salary', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));
    mockEmployeeService.updateEmployeeSalary.and.returnValue(of(mockEmployee));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    // Open dialog and start update
    component.openEditDialog();
    component.newSalary.set(60000);
    component.updating.set(true);
    fixture.detectChanges();

    const saveButton = fixture.debugElement.query(By.css('.btn-primary'));
    expect(saveButton.nativeElement.textContent.trim()).toContain('Saving...');
    expect(saveButton.nativeElement.disabled).toBeTrue();
  }));

  it('should navigate back when back button is clicked', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const backButton = fixture.debugElement.query(By.css('.btn-outline-primary'));
    backButton.nativeElement.click();

    expect(mockRouter.navigate).toHaveBeenCalledWith(['/']);
  }));

  it('should handle error when updating salary fails', fakeAsync(() => {
    mockEmployeeService.getEmployee.and.returnValue(of(mockEmployee));
    mockTaxService.calculateTax.and.returnValue(of(mockTaxCalculation));
    mockEmployeeService.updateEmployeeSalary.and.returnValue(throwError(() => new Error('Update failed')));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    // Open dialog and try to update
    component.openEditDialog();
    component.newSalary.set(60000);
    fixture.detectChanges();

    const saveButton = fixture.debugElement.query(By.css('.btn-primary'));
    saveButton.nativeElement.click();
    tick();
    fixture.detectChanges();

    expect(component.error()).toBe('Failed to update salary. Please try again.');
    expect(component.updating()).toBeFalse();
  }));
}); 