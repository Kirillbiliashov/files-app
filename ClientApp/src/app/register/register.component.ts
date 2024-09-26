import { Component } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, NgModel, Validators } from '@angular/forms';
import { RegisterUser } from '../models/register-user';
import { AuthHttpService } from '../services/auth-service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  signUpAttempted = false;
  showPassword = false;
  signingUp = false;
  userForm: FormGroup;

  constructor(private authService: AuthHttpService, private router: Router, private fb: FormBuilder) {
    this.userForm = fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&._])[A-Za-z\\d@$!%*?&._]+$')
        ]
      ]
    });
  }

  signUp() {
    this.signUpAttempted = true;
    if (this.userForm.valid) {
      this.signingUp = true;
      const { firstName, lastName, email, password } = this.userForm.value;
      const newUser = new RegisterUser(firstName, lastName, email, password);
      this.authService.register(newUser).pipe(
        finalize(() => this.signingUp = false)
      )
        .subscribe({
          next: (user) => {
            localStorage.setItem('currentUser', JSON.stringify(user))
            this.router.navigate(['/'])
          },
          error: console.log
        })
    } else {
      console.log('Form is invalid');
    }
  }

  isControlInvalid(controlName: string): boolean {
    const control = this.userForm.get(controlName);
    return control ? control.invalid && (control.touched || this.signUpAttempted) : false;
  }

}
