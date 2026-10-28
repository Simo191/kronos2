import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EstimationDetailComponent } from './estimation-detail.component';

describe('EstimationDetailComponent', () => {
  let component: EstimationDetailComponent;
  let fixture: ComponentFixture<EstimationDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstimationDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EstimationDetailComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
