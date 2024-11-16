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
  }

  get enclosingFolderName(): string | null {
    if (!this.fodlerData || !this.fodlerData.folder) return null
    return this.fodlerData.folder.nameIdx == 0 ? this.fodlerData.folder.name : this.fodlerData.folder.name + ' (' + this.fodlerData.folder.nameIdx + ')'
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

}
