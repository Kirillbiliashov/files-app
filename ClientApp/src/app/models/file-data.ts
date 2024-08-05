import { UserFile } from "./user-file";
import { UserFolder } from "./user-folder";

export class FileData {
    constructor(
        public folders: UserFolder[],
        public files: UserFile[]
    ) {}
}