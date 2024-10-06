import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthHttpService } from '../services/auth-service';

@Component({
  selector: 'app-google-auth-callback',
  templateUrl: './google-auth-callback.component.html',
  styleUrls: ['./google-auth-callback.component.css']
})
export class GoogleAuthCallbackComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private authService: AuthHttpService,
    private router: Router
  ) { }


  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const authCode = params['code'];
      console.log(`auth code: ${authCode}`);
      this.authService.processGoogleLogin(authCode).subscribe({
        next: (user) => {
          localStorage.setItem('currentUser', JSON.stringify(user))
          this.router.navigate(['/'])
        },
        error: console.log
      })
    });
  }

}
