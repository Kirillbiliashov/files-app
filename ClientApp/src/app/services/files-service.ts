import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserFile } from "../models/user-file";
import { FileData } from "../models/file-data";

@Injectable({
    providedIn: 'root'
  })
export class FilesHttpService {

    constructor(private client: HttpClient) { }

    getFiles() {
        return this.client.get<FileData>('api/files')
    }

    uploadFiles(files: FileList) {
        const formData = new FormData();

        let folderName = null;

        for (let i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
            formData.append(`paths_${i}`, files[i].webkitRelativePath);
        }

        return this.client.post('api/files/upload', formData)
    }
}