import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { FoldersHttpService } from '../services/folders-service';
import { SidebarFolder } from '../models/sidebar-folder';

@Component({
  selector: 'app-page-layout',
  templateUrl: './page-layout.component.html',
  styleUrls: ['./page-layout.component.css']
})
export class PageLayoutComponent implements OnInit {
  @Input() tableItems: any[] = [];
  @Input() enclosingFolderName: string | null = null;

  @Output() fileUpload = new EventEmitter();

  folders: SidebarFolder[] = [];
  infoMessage = '';
  showDragBorder = false;
  droppedFiles: FileList | null = null;

  constructor(private foldersService: FoldersHttpService) { }

  ngOnInit(): void {
    this.foldersService.getAllFolders().subscribe({
      next: (res) => {
        res.forEach(f => f.name = f.nameIdx == 0 ? f.name : f.name + ' (' + f.nameIdx + ')');
        this.folders = res.map(f => new SidebarFolder(f));
      },
      error: console.log
    })
  }

  onFileUpload() {
    this.fileUpload.emit();
  }

  onInfoChange(message: string) {
    this.infoMessage = message;
  }
  

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    if (!this.showDragBorder) {
      this.showDragBorder = true;
    }
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.showDragBorder = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.showDragBorder = false;
    if (event.dataTransfer?.files) {
      this.droppedFiles = event.dataTransfer.files;
    }
  }

}
