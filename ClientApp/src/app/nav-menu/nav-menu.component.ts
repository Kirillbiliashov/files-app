import { Component, OnDestroy, OnInit } from '@angular/core';
import { CookieUser } from '../models/cookie-user';
import { AuthHttpService } from '../services/auth-service';
import { Router } from '@angular/router';
import { UserService } from '../services/user-service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnDestroy {
  currentUser: CookieUser | null = null;
  private subscription: Subscription;

  isExpanded = false;

  constructor(
    private authService: AuthHttpService,
    private router: Router,
    private userService: UserService) {
    this.subscription = this.userService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }


  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.userService.setCurrentUser(null);
      localStorage.removeItem('currentUser');
      this.currentUser = null;
      this.router.navigate(['/login'])
    })
  }

}
