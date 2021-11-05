import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MomentListComponent } from './moment-list.component';

describe('MomentListComponent', () => {
  let component: MomentListComponent;
  let fixture: ComponentFixture<MomentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MomentListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MomentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
