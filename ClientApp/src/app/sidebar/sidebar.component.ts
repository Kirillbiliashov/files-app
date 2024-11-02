import { Component, Input, OnInit } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { SidebarFolder } from '../models/sidebar-folder';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  @Input() folders!: UserFolder[];
  foldersListDisplayed = true;

  sidebarFolders!: SidebarFolder[];

  ngOnInit(): void {
    this.sidebarFolders = this.folders.map(f => new SidebarFolder(f));
  }
}
