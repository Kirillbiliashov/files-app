import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { RegisterUser } from "../models/register-user";

@Injectable({providedIn: 'root'})
export class AuthHttpService {
    constructor(private client: HttpClient) {}

    register(user: RegisterUser) {
        return this.client.post('api/auth/register', user);
    }
}