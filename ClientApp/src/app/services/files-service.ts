import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserFile } from "../models/user-file";
import { FileData } from "../models/file-data";
import { SelectedItem } from "../models/selected-item";

@Injectable({
    providedIn: 'root'
  })
export class FilesHttpService {

    constructor(private client: HttpClient) { }

    getFiles() {
        return this.client.get<FileData>('api/files')
    }

    uploadFiles(files: FileList, folder: string | null = null) {
        const formData = new FormData();

        if (folder) {
            formData.append('folder', folder)
        }

        for (let i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
            formData.append(`lastModified_${i}`, files[i].lastModified.toString());
        }

        return this.client.post('api/files/upload', formData)
    }

    createFileLink(fileId: string) {
        return this.client.post<{linkId: string}>(`api/shared-links/create/${fileId}`, {});
    }

}