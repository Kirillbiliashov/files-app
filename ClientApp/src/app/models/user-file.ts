import { Item } from "./item";

export class UserFile extends Item {
    constructor(
        public id: string,
        public name: string,
        public size: number,
        public lastModified: number,
        public isHovered: boolean = false,
        public isStarred: boolean
    ) {
        super();
    }

}