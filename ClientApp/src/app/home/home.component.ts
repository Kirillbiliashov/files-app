import { Component, OnInit } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { UserFile } from '../models/user-file';
import { FileData } from '../models/file-data';
import { FoldersHttpService } from '../services/folders-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  folderName: string = "";
  public data: FileData | null = null;
  infoMessage = '';
  showDragBorder = false;
  droppedFiles: FileList | null = null;

  constructor(private filesService: FilesHttpService, private router: Router) { }

  ngOnInit(): void {
    this.loadFiles();
  }

  loadFiles() {
    this.filesService.getFiles().subscribe(data => this.data = data);
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
