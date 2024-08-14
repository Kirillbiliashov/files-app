import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { SelectedItem } from "../models/selected-item";

@Injectable({
    providedIn: 'root'
})
export class ItemsHttpService {
    constructor(private client: HttpClient) { }

    deleteItems(items: SelectedItem[]) {
        const body = { items }
        return this.client.post('api/items/delete', body);
    }

    downloadItems(items: SelectedItem[]) {
        const body = { items }
        const httpOptions = {
            responseType: 'blob' as const
        };

        return this.client.post('api/items/download', body, httpOptions);
    }

    starItem(item: SelectedItem) {
        return this.client.patch('api/items/star', item);
    }

    unstarItem(item: SelectedItem) {
        return this.client.patch('api/items/unstar', item);
    }

}