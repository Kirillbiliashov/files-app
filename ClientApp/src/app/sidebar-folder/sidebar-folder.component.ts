import { Component, Input } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { FoldersHttpService } from '../services/folders-service';
import { SidebarFolder } from '../models/sidebar-folder';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar-folder',
  templateUrl: './sidebar-folder.component.html',
  styleUrls: ['./sidebar-folder.component.css']
})
export class SidebarFolderComponent {
  @Input() folder!: SidebarFolder;

  constructor(private foldersService: FoldersHttpService, private router: Router) {}

  handleFolderClick(event: Event) {
    event.stopPropagation();
    this.folder.expanded = !this.folder.expanded;
    if (this.folder.expanded && !this.folder.retrievedSubfolders) {
      this.loadSubfolders();
    }
  }

  loadSubfolders() {
    this.foldersService.getSubfolders(this.folder.folder.id).subscribe({
      next: (res) => {
        this.folder.subfolders = res.map(f => new SidebarFolder(f));
        this.folder.retrievedSubfolders = true;
      },
      error: console.log
    })
  }

  navigateToFolder(id: string) {
    this.router.navigate(['/folders', id]);
  }


}
