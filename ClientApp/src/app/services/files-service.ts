import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserFile } from "../models/user-file";

@Injectable({
    providedIn: 'root'
  })
export class FilesHttpService {

    constructor(private client: HttpClient) { }

    getFiles() {
        return this.client.get<UserFile[]>('api/files')
    }

    uploadFiles(files: FileList) {
        const formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
        }

        return this.client.post('api/files/upload', formData)
    }
}