import { LoginUser } from "./login-user";

export class RegisterUser extends LoginUser {

    constructor(
        private firstName: string,
        private lastName: string,
        email: string,
        password: string) {
        super(email, password);
    }

}