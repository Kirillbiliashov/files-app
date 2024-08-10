import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FoldersHttpService } from '../services/folders-service';
import { FolderData } from '../models/folder-data';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css']
})
export class FolderComponent implements OnInit {
  folderName: string = "";
  folderId: string | undefined;
  fodlerData: FolderData | undefined;

  constructor(private route: ActivatedRoute, private foldersService: FoldersHttpService, private router: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.folderId = params['id'];
      this.loadFolderData();
    });
  }

  loadFolderData() {
    if (this.folderId) {
      this.foldersService.getFolderData(this.folderId)
      .subscribe(data => this.fodlerData = data);
    }
  }

  createFolder() {
    this.foldersService.createFolder(this.folderName, this.folderId as string).subscribe(res => {
      if (res.folderId) {
        this.router.navigate(['/folders', res.folderId]);
      }
    })
  }

}
