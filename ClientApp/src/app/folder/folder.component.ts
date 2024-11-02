import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FoldersHttpService } from '../services/folders-service';
import { FolderData } from '../models/folder-data';
import { UserFolder } from '../models/user-folder';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css']
})
export class FolderComponent implements OnInit {
  folderName: string = "";
  folderId: string | undefined;
  fodlerData: FolderData | undefined;
  folders: UserFolder[] = [];

  constructor(private route: ActivatedRoute, private foldersService: FoldersHttpService, private router: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.folderId = params['id'];
      this.loadFolderData();
    });
    this.foldersService.getAllFolders().subscribe({
      next: (res) => {
        res.forEach(f => f.name = f.nameIdx == 0 ? f.name : f.name + ' (' + f.nameIdx + ')');
        this.folders = res
      },
      error: console.log
    })
  }

  get enclosingFolderName(): string {
    return this.fodlerData?.folder?.nameIdx == 0 ? this.fodlerData?.folder?.name : this.fodlerData?.folder?.name + ' (' + this.fodlerData?.folder?.nameIdx + ')'
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
