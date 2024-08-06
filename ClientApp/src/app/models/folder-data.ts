import { UserFile } from "./user-file";
import { UserFolder } from "./user-folder";

export class FolderData {
    constructor(
        public folder: UserFolder,
        public files: UserFile[]
    ) {}
}