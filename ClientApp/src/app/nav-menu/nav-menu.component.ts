import { Component, OnInit } from '@angular/core';
import { CookieUser } from '../models/cookie-user';
import { AuthHttpService } from '../services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  currentUser: CookieUser | null = null;

  constructor(private authService: AuthHttpService, private router: Router) { }

  ngOnInit(): void {
    const currentUserStr = localStorage.getItem('currentUser');
    this.currentUser = currentUserStr ? JSON.parse(currentUserStr) : null;
  }

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authService.logout().subscribe(() => {
      localStorage.removeItem('currentUser');
      this.currentUser = null;
      this.router.navigate(['/login'])
    })
  }

}
