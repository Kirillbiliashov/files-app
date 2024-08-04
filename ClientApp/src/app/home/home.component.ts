import { Component, OnInit } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { UserFile } from '../models/user-file';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public files: UserFile[] = [];

  constructor(private filesService: FilesHttpService) { }
  
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
      .subscribe(files => {
        this.files = files;
      });
  }

}
