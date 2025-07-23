import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateSalaryDialogComponent } from './update-salary-dialog.component';

describe('UpdateSalaryDialogComponent', () => {
  let component: UpdateSalaryDialogComponent;
  let fixture: ComponentFixture<UpdateSalaryDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateSalaryDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateSalaryDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
