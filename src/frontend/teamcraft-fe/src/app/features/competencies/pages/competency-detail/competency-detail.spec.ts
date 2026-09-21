import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompetencyDetail } from './competency-detail';

describe('CompetencyDetail', () => {
  let component: CompetencyDetail;
  let fixture: ComponentFixture<CompetencyDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompetencyDetail],
    }).compileComponents();

    fixture = TestBed.createComponent(CompetencyDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
