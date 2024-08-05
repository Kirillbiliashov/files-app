import { Component, OnInit } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { UserFile } from '../models/user-file';
import { GroupedFiles } from '../models/grouped-files';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public groupedFiles: GroupedFiles[] = [];

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
        console.log(files)
        this.groupedFiles = files;
      });
  }

  getFolderSize(group: GroupedFiles) {
    return group.files.map(f => f.length).reduce((a, b) => a + b, 0);
  }

  getFolderModified(group: GroupedFiles) {
    return group.files.map(f => f.modified).reduce((a, b) => a > b? a : b);
  }

}
