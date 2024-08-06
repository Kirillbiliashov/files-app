import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FoldersHttpService } from '../services/folders-service';
import { FolderData } from '../models/folder-data';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css']
})
export class FolderComponent implements OnInit {
  folderId: string | undefined;
  fodlerData: FolderData | undefined;

  constructor(private route: ActivatedRoute, private foldersService: FoldersHttpService) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.folderId = params['id'];
      this.loadFolderData();
    });
  }

  loadFolderData() {
    if (this.folderId) {
      this.foldersService.getFolderData(this.folderId).subscribe(data => this.fodlerData = data);
    }
  }

  uploadFile(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {

      // this.filesService.uploadFiles(input.files)
      //   .subscribe(_ => this.loadFiles())
    }
  }

  openFileChooser(fileInput: HTMLInputElement) {
    fileInput.click();
  }

}
