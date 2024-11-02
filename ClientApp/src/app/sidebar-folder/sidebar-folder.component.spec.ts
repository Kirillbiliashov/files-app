import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SidebarFolderComponent } from './sidebar-folder.component';

describe('SidebarFolderComponent', () => {
  let component: SidebarFolderComponent;
  let fixture: ComponentFixture<SidebarFolderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SidebarFolderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SidebarFolderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
