export class SelectedItem {
    constructor(public type: string, public id: string) { }

    isEqual(other: SelectedItem): boolean {
        return this.type === other.type && this.id === other.id;
    }

}