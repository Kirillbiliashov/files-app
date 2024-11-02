import { UserFolder } from "./user-folder";

export class SidebarFolder {
    subfolders: SidebarFolder[] = [];
    expanded = false;
    retrievedSubfolders = false;
    constructor(
        public folder: UserFolder
    ) {}

}