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
            console.log(files[i].lastModified)
            formData.append('files', files[i]);
            formData.append(`lastModified_${i}`, files[i].lastModified.toString());
        }

        return this.client.post('api/files/upload', formData)
    }
}