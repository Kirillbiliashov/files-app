import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserFile } from "../models/user-file";
import { GroupedFiles } from "../models/grouped-files";

@Injectable({
    providedIn: 'root'
  })
export class FilesHttpService {

    constructor(private client: HttpClient) { }

    getFiles() {
        return this.client.get<GroupedFiles[]>('api/files')
    }

    uploadFiles(files: FileList) {
        const formData = new FormData();

        let folderName = null;

        for (let i = 0; i < files.length; i++) {
            console.log(files[i].lastModified)
            formData.append('files', files[i]);
            console.log(`realtive path: ${files[i].webkitRelativePath}`)
            formData.append(`lastModified_${i}`, files[i].lastModified.toString());
            if (files[i].webkitRelativePath && !folderName) {
                folderName = files[i].webkitRelativePath.split("/").shift();
                formData.append('folder', folderName as string);
            }
        }

        return this.client.post('api/files/upload', formData)
    }
}