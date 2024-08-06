import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserFile } from "../models/user-file";
import { FileData } from "../models/file-data";
import { FolderData } from "../models/folder-data";

@Injectable({
    providedIn: 'root'
  })
export class FoldersHttpService {

    constructor(private client: HttpClient) { }

    getFolderData(folderId: string) {
        return this.client.get<FolderData>('api/folders/' + folderId)
    }


}