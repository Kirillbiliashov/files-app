import { Component, Input } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { UserFile } from '../models/user-file';

@Component({
  selector: 'app-files-table',
  templateUrl: './files-table.component.html',
  styleUrls: ['./files-table.component.css']
})
export class FilesTableComponent {
  @Input() folders: UserFolder[] = [];

  @Input() files: UserFile[] = [];
}
