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

  constructor(private filesService: FilesHttpService, private router: Router) { }

  ngOnInit(): void {
    this.loadFiles();
  }

  loadFiles() {
    this.filesService.getFiles()
      .subscribe({
        next: data => this.data = data,
        error: e =>  this.router.navigate(['/register']) 
      });
  }

}
