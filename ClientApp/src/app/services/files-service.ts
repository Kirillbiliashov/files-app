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

        for (let i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
            formData.append(`lastModified_${i}`, files[i].lastModified.toString());
        }

        return this.client.post('api/files/upload', formData)
    }

    deleteFiles(fileIds: string[]) {
        const body = {
            files: fileIds
        }

        return this.client.post('api/files/delete', body);
    }
}