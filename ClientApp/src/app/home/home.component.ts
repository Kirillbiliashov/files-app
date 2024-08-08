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

  constructor(private filesService: FilesHttpService, 
    private foldersService: FoldersHttpService, 
    private router: Router) { }

  ngOnInit(): void {
    this.loadFiles();
  }

  uploadFile(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {

      this.filesService.uploadFiles(input.files)
        .subscribe(_ => this.loadFiles())
    }
  }

  openFileChooser(fileInput: HTMLInputElement) {
    fileInput.click();
  }

  loadFiles() {
    this.filesService.getFiles()
      .subscribe(data => {
        this.data = data;
      });
  }

  createFolder() {
    this.foldersService.createFolder(this.folderName, null).subscribe(res => {
      if (res.folderId) {
        this.router.navigate(['/folders', res.folderId]);
      }
    })
  }

}
