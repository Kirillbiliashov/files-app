import { Component, OnInit } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { UserFile } from '../models/user-file';
import { FileData } from '../models/file-data';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public data: FileData | null = null;

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
      .subscribe(data => {
        this.data = data;
      });
  }

}
