import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { CookieUser } from "../models/cookie-user";

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private currentUserSubject: BehaviorSubject<CookieUser | null>;
    public currentUser$: Observable<CookieUser | null>;

    constructor() {
        const currentUserStr = localStorage.getItem('currentUser');
        const currentUser = currentUserStr ? JSON.parse(currentUserStr) : null;
        this.currentUserSubject = new BehaviorSubject<CookieUser | null>(currentUser);
        this.currentUser$ = this.currentUserSubject.asObservable();
    }

    setCurrentUser(user: CookieUser | null): void {
        if (user) {
            localStorage.setItem('currentUser', JSON.stringify(user));
        } else {
            localStorage.removeItem('currentUser');
        }
        this.currentUserSubject.next(user);
    }

}