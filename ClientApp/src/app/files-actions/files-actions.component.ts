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
  @Output() onInfoChange = new EventEmitter();
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
      const fileNames: string[] = [];
      for (let i = 0; i < input.files.length; i++) {
        fileNames.push(input.files[i].name);
      }
      this.onInfoChange.emit(`Uploading files: ${fileNames.join(', ')}`)
      this.filesService.uploadFiles(input.files, this.folderId)
        .subscribe(_ => {
          this.onFileUpload.emit();
          this.onInfoChange.emit(`Files uploaded.`);
          setTimeout(() => this.onInfoChange.emit(''), 3000);
        })
    }
  }

  openFileChooser(fileInput: HTMLInputElement) {
    fileInput.click();
  }

  createFolder() {
    this.foldersService.createFolder(this.folderName, this.folderId).subscribe(res => {
      if (res.folderId) {
        this.folderName = "";
        this.router.navigate(['/folders', res.folderId]);
      }
    })
  }

}
