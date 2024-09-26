import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { RegisterUser } from "../models/register-user";
import { LoginUser } from "../models/login-user";
import { CookieUser } from "../models/cookie-user";

@Injectable({providedIn: 'root'})
export class AuthHttpService {
    constructor(private client: HttpClient) {}

    register(user: RegisterUser) {
        return this.client.post<CookieUser>('api/auth/register', user);
    }

    login(user: LoginUser) {
        return this.client.post<CookieUser>('api/auth/login', user);
    }

    logout() {
        return this.client.get('api/auth/logout');
    }
}