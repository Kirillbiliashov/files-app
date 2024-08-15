import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { FoldersHttpService } from '../services/folders-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-files-actions',
  templateUrl: './files-actions.component.html',
  styleUrls: ['./files-actions.component.css']
})
export class FilesActionsComponent {
  @Output() onFileUpload = new EventEmitter();
  folderName: string = "";


  constructor(private filesService: FilesHttpService,
    private foldersService: FoldersHttpService,
    private router: Router,
    private route: ActivatedRoute) {
  }

  get folderId() {
    return this.route.snapshot.routeConfig?.path == "folders/:id" ? this.route.snapshot.paramMap.get('id') : null
  }

  uploadFile(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      this.filesService.uploadFiles(input.files, this.folderId)
        .subscribe(_ => this.onFileUpload.emit())
    }
  }

  openFileChooser(fileInput: HTMLInputElement) {
    fileInput.click();
  }

  createFolder() {
    this.foldersService.createFolder(this.folderName, this.folderId).subscribe(res => {
      if (res.folderId) {
        this.router.navigate(['/folders', res.folderId]);
      }
    })
  }

}
