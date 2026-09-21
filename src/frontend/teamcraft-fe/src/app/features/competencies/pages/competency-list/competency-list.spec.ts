import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompetencyList } from './competency-list';

describe('CompetencyList', () => {
  let component: CompetencyList;
  let fixture: ComponentFixture<CompetencyList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompetencyList],
    }).compileComponents();

    fixture = TestBed.createComponent(CompetencyList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
