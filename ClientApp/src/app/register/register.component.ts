import { Component } from '@angular/core';
import { NgForm, NgModel } from '@angular/forms';
import { RegisterUser } from '../models/register-user';
import { AuthHttpService } from '../services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  showPassword = false;
  signUpAttempted = false;

  constructor(private authService: AuthHttpService, private router: Router) {}

  signUp(form: NgForm) {
    this.signUpAttempted = true;
    if (form.valid) {
      const { firstName, lastName, email, password } = form.value;
      const newUser = new RegisterUser(firstName, lastName, email, password);
      this.authService.register(newUser).subscribe({
        next: () => this.router.navigate(['/']),
        error: console.log
      })
      console.log(newUser); // Handle user registration logic here
    } else {
      console.log('Form is invalid');
    }
  }

  isInputInvalid = (input: NgModel) => input.invalid && input.touched && this.signUpAttempted;

}
