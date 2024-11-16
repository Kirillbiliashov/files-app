import { Component, Input, OnInit } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { SidebarFolder } from '../models/sidebar-folder';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent  {
  @Input() folders!: SidebarFolder[];
  foldersListDisplayed = true;

}
