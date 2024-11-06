import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FilesHttpService } from '../services/files-service';
import { FoldersHttpService } from '../services/folders-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-files-actions',
  templateUrl: './files-actions.component.html',
  styleUrls: ['./files-actions.component.css']
})
export class FilesActionsComponent implements OnChanges {
  @Input() droppedFiles: FileList | null = null;
  @Output() onFileUpload = new EventEmitter();
  @Output() onInfoChange = new EventEmitter();
  folderName: string = "";


  constructor(private filesService: FilesHttpService,
    private foldersService: FoldersHttpService,
    private router: Router,
    private route: ActivatedRoute) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.droppedFiles && changes.droppedFiles.currentValue) {
      this.uploadFiles(this.droppedFiles!);
    }
  }

  get folderId() {
    return this.route.snapshot.routeConfig?.path == "folders/:id" ? this.route.snapshot.paramMap.get('id') : null
  }

  uploadFile(event: Event) {
    const input = event.target as HTMLInputElement;
    this.uploadFiles(input.files!);
  }

  uploadFiles(files: FileList) {
    if (files && files.length) {
      const fileNames: string[] = [];
      for (let i = 0; i < files.length; i++) {
        fileNames.push(files[i].name);
      }
      this.onInfoChange.emit(`Uploading files: ${fileNames.join(', ')}`)
      this.filesService.uploadFiles(files, this.folderId)
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
