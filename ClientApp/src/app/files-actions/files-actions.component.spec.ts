import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilesActionsComponent } from './files-actions.component';

describe('FilesActionsComponent', () => {
  let component: FilesActionsComponent;
  let fixture: ComponentFixture<FilesActionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilesActionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilesActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
