import { Component, OnInit } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { UserFile } from '../models/user-file';
import { FileData } from '../models/file-data';
import { FoldersHttpService } from '../services/folders-service';
import { Router } from '@angular/router';
import { Item } from '../models/item';
import { UserFolder } from '../models/user-folder';
import { UserService } from '../services/user-service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  folderName: string = "";
  public tableItems: Item[] = [];
  infoMessage = '';
  showDragBorder = false;
  droppedFiles: FileList | null = null;

  constructor(private filesService: FilesHttpService, private router: Router, private userService: UserService) {
    const currentUserStr = localStorage.getItem('currentUser');
    const currentUser = currentUserStr ? JSON.parse(currentUserStr) : null;
    userService.setCurrentUser(currentUser);
  }

  ngOnInit(): void {
    this.loadFiles();
  }

  loadFiles() {
    this.filesService.getFiles().subscribe(data => {
      this.tableItems = [];
      data.folders.forEach(f => {
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
