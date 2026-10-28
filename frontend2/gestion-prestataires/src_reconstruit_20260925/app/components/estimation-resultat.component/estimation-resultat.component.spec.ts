import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EstimationResultatComponent } from './estimation-resultat.component';

describe('EstimationResultatComponent', () => {
  let component: EstimationResultatComponent;
  let fixture: ComponentFixture<EstimationResultatComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstimationResultatComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EstimationResultatComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
