import { Component, OnInit } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { UserFile } from '../models/user-file';
import { FileData } from '../models/file-data';
import { FoldersHttpService } from '../services/folders-service';
import { Router } from '@angular/router';
import { Item } from '../models/item';
import { UserFolder } from '../models/user-folder';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  folderName: string = "";
  public tableItems: Item[] = [];
  folders: UserFolder[] = [];
  infoMessage = '';
  showDragBorder = false;
  droppedFiles: FileList | null = null;

  constructor(private filesService: FilesHttpService, private router: Router) { }

  ngOnInit(): void {
    this.loadFiles();
  }

  loadFiles() {
    console.log(`updaing files`);
    this.filesService.getFiles().subscribe(data => {
      this.tableItems = [];
      this.folders = data.folders;
      this.folders.forEach(f => {
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

  onNewInfo(message: string) {
    this.infoMessage = message
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
