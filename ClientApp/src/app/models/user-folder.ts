import { Item } from "./item";

export class UserFolder extends Item {
    constructor(
        public id: string,
        public name: string,
        public size: number,
        public lastModified: number,
        public isStarred: boolean
    ) {
        super();
    }
}