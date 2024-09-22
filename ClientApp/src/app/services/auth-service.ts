import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { RegisterUser } from "../models/register-user";
import { LoginUser } from "../models/login-user";

@Injectable({providedIn: 'root'})
export class AuthHttpService {
    constructor(private client: HttpClient) {}

    register(user: RegisterUser) {
        return this.client.post('api/auth/register', user);
    }

    login(user: LoginUser) {
        return this.client.post('api/auth/login', user);
    }
}