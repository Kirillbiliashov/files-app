import { Component } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, NgModel, Validators } from '@angular/forms';
import { LoginUser } from '../models/login-user';
import { AuthHttpService } from '../services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  signUpAttempted = false;
  showPassword = false;
  userForm: FormGroup;

  constructor(private authService: AuthHttpService, private router: Router, private fb: FormBuilder) {
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
      const { email, password } = this.userForm.value;
      const user = new LoginUser(email, password);
      this.authService.login(user).subscribe({
        next: () => this.router.navigate(['/']),
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
