import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FoldersHttpService } from '../services/folders-service';
import { FolderData } from '../models/folder-data';
import { UserFolder } from '../models/user-folder';
import { Item } from '../models/item';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css']
})
export class FolderComponent implements OnInit {
  folderName: string = "";
  folderId: string | undefined;
  fodlerData: FolderData | undefined;
  folders: UserFolder[] = [];
  showDragBorder = false;
  droppedFiles: FileList | null = null;
  infoMessage = '';
  tableItems: Item[] = [];

  constructor(private route: ActivatedRoute, private foldersService: FoldersHttpService, private router: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.folderId = params['id'];
      this.loadFolderData();
    });
    this.foldersService.getAllFolders().subscribe({
      next: (res) => {
        res.forEach(f => f.name = f.nameIdx == 0 ? f.name : f.name + ' (' + f.nameIdx + ')');
        this.folders = res
      },
      error: console.log
    })
  }

  get enclosingFolderName(): string {
    return this.fodlerData?.folder?.nameIdx == 0 ? this.fodlerData?.folder?.name : this.fodlerData?.folder?.name + ' (' + this.fodlerData?.folder?.nameIdx + ')'
  }

  loadFolderData() {
    if (this.folderId) {
      this.foldersService.getFolderData(this.folderId)
        .subscribe(data => {
          this.tableItems = [];
          this.fodlerData = data;
          data.subfolders.forEach(f => {
            f.type = 'folder';
            f.name = f.nameIdx == 0 ? f.name : f.name + ' (' + f.nameIdx + ')';
            this.tableItems.push(f)
          });
          data.files.forEach(f => {
            f.type = 'file';
            this.tableItems.push(f)
          });
        });
    }
  }

  createFolder() {
    this.foldersService.createFolder(this.folderName, this.folderId as string).subscribe(res => {
      if (res.folderId) {
        this.router.navigate(['/folders', res.folderId]);
      }
    })
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
    console.log(`dropped`);
    event.preventDefault();
    event.stopPropagation();
    this.showDragBorder = false;
    if (event.dataTransfer?.files) {
      this.droppedFiles = event.dataTransfer.files;
    }
  }

  onNewInfo(message: string) {
    this.infoMessage = message
  }

}
