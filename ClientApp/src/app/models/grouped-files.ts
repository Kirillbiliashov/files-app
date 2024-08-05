import { UserFile } from "./user-file";

export class GroupedFiles {
    constructor(
    public folder: string | undefined,
    public files: UserFile[]) {}
}