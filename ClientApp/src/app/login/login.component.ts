import { Component } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, NgModel, Validators } from '@angular/forms';
import { LoginUser } from '../models/login-user';
import { AuthHttpService } from '../services/auth-service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { UserService } from '../services/user-service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  signUpAttempted = false;
  showPassword = false;
  loggingIn = false;
  userForm: FormGroup;

  constructor(
    private authService: AuthHttpService,
    private router: Router,
    private fb: FormBuilder,
    private userService: UserService) {
    this.userForm = fb.group({
      email: ['', [Validators.required, Validators.email]],
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

  login() {
    this.signUpAttempted = true;
    if (this.userForm.valid) {
      this.loggingIn = true;
      const { email, password } = this.userForm.value;
      const user = new LoginUser(email, password);
      this.authService.login(user).pipe(
        finalize(() => this.loggingIn = false)
      ).subscribe({
        next: (user) => {
          this.userService.setCurrentUser(user);
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

  loginWithGoogle() {
    window.location.href = '/api/auth/login/google';
  }

}
