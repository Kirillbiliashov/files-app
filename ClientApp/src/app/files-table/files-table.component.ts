import { Component, Input, EventEmitter, Output } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { UserFile } from '../models/user-file';
import { FilesHttpService } from '../services/files-service';

@Component({
  selector: 'app-files-table',
  templateUrl: './files-table.component.html',
  styleUrls: ['./files-table.component.css']
})
export class FilesTableComponent {
  @Input() folders: UserFolder[] = [];
  @Input() files: UserFile[] = [];
  @Output() onFilesChange = new EventEmitter();

  lastHoverIdx: number = -1;
  selectedFileIds: string[] = [];

  constructor(private filesService: FilesHttpService) {}

  changeFilesSelection(checked: boolean, file: UserFile) {
    if (checked) {
      this.selectedFileIds.push(file.id);
    } else {
      const idx = this.selectedFileIds.indexOf(file.id);
      if (idx > -1) {
        this.selectedFileIds.splice(idx, 1);
      }
    }
  }

  selectSingleRow(event: Event, fileId: string) {
    console.log('selectSingleRow')
    event.stopPropagation();
    this.selectedFileIds = []; 
    this.selectedFileIds.push(fileId);
  }

  deleteFiles() {
    this.filesService.deleteFiles(this.selectedFileIds).subscribe({
      next: () => {
        this.selectedFileIds = [];
        this.onFilesChange.emit();
      },
      error: () => console.log('error')
    });
  }
}
