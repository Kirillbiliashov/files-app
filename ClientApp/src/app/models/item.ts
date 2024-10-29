export abstract class Item {
    abstract id: string;
    abstract userId: string;
    abstract  name: string;
    abstract size: number;
    abstract lastModified: number;
    abstract isStarred: boolean;
    type?: string;
}