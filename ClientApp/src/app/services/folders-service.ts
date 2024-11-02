import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserFile } from "../models/user-file";
import { FileData } from "../models/file-data";
import { FolderData } from "../models/folder-data";
import { CreateFolderResponse } from "../models/create-folder-response";
import { UserFolder } from "../models/user-folder";

@Injectable({
    providedIn: 'root'
})
export class FoldersHttpService {

    constructor(private client: HttpClient) { }

    getAllFolders() {
        return this.client.get<UserFolder[]>('api/folders');
    }

    getFolderData(folderId: string) {
        return this.client.get<FolderData>('api/folders/' + folderId)
    }

    createFolder(name: string, folderId: string | null) {
        const body = { folderId, name }
        return this.client.post<CreateFolderResponse>('api/folders', body)
    }

    getSubfolders(folderId: string) {
        return this.client.get<UserFolder[]>(`api/folders/subfolders/${folderId}`)
    }

}