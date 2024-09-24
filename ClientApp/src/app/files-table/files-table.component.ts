import { Component, Input, EventEmitter, Output, OnInit } from '@angular/core';
import { UserFolder } from '../models/user-folder';
import { UserFile } from '../models/user-file';
import { FilesHttpService } from '../services/files-service';
import { SelectedItem } from '../models/selected-item';
import { ItemsHttpService } from '../services/items-service';
import { Item } from '../models/item';

@Component({
  selector: 'app-files-table',
  templateUrl: './files-table.component.html',
  styleUrls: ['./files-table.component.css']
})
export class FilesTableComponent {
  @Input() folders: UserFolder[] = [];
  @Input() files: UserFile[] = [];
  @Output() onFilesChange = new EventEmitter();
  @Output() onInfoChange = new EventEmitter();

  lastHoverIdx: number = -1;
  headerHovered: boolean = false;
  selectedTableItems: SelectedItem[] = [];
  showStarredOnly: boolean = false;

  constructor(private filesService: FilesHttpService, private itemsService: ItemsHttpService) { }


  changeSelection(checked: boolean, type: string, id: string) {
    if (checked) {
      this.selectedTableItems.push(new SelectedItem(type, id));
    } else {
      const idx = this.selectedTableItems.findIndex(i => i.id == id && i.type == type)
      if (idx > -1) {
        this.selectedTableItems.splice(idx, 1);
      }
    }
  }

  rowSelected = (id: string) => this.selectedTableItems.some(i => i.id == id);

  changeAllFilesSelection(checked: boolean) {
    this.selectedTableItems = [];
    if (checked) {
      this.folders.forEach(f => this.selectedTableItems.push(new SelectedItem("folder", f.id)));
      this.files.forEach(f => this.selectedTableItems.push(new SelectedItem("file", f.id)));
    }
  }

  selectSingleRow(event: Event, type: string, id: string) {
    event.stopPropagation();
    this.selectedTableItems = [];
    this.selectedTableItems.push(new SelectedItem(type, id));
  }

  deleteFiles() {
    this.onInfoChange.emit(`Deleting ${this.selectedTableItems.length} items...`);
    this.itemsService.deleteItems(this.selectedTableItems).subscribe({
      next: () => {
        this.onInfoChange.emit(`Successfully deleted ${this.selectedTableItems.length} items.`);
        setTimeout(() => this.onInfoChange.emit(''), 3000);
        this.selectedTableItems = [];
        this.onFilesChange.emit();
      },
      error: () => console.log('error')
    });
  }

  downloadFiles() {
    this.onInfoChange.emit(`Downloading items...`);
    this.itemsService.downloadItems(this.selectedTableItems).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'files.zip';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.onInfoChange.emit('');
      },
      error: () => { }
    });
  }

  changeStarredState(item: Item, type: string) {
    const selectedItem = new SelectedItem(type, item.id);
    console.log('clicked')
    if (item.isStarred) {
      this.itemsService.unstarItem(selectedItem).subscribe({
        next: () => item.isStarred = false
      });
    } else {
      this.itemsService.starItem(selectedItem).subscribe({
        next: () => item.isStarred = true
      });
    }
  }

}
