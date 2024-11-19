import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FolderComponent } from './folder/folder.component';
import { FilesTableComponent } from './files-table/files-table.component';
import { FilesActionsComponent } from './files-actions/files-actions.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { GoogleAuthCallbackComponent } from './google-auth-callback/google-auth-callback.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { SidebarFolderComponent } from './sidebar-folder/sidebar-folder.component';
import { PageLayoutComponent } from './page-layout/page-layout.component';
import { FormatBytesPipe } from './pipes/format-bytes-pipe';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FolderComponent,
    FilesTableComponent,
    FilesActionsComponent,
    RegisterComponent,
    LoginComponent,
    GoogleAuthCallbackComponent,
    SidebarComponent,
    SidebarFolderComponent,
    PageLayoutComponent,
    FormatBytesPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'folders/:id', component: FolderComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'auth/google/callback', component: GoogleAuthCallbackComponent },
      { path: 'login', component: LoginComponent },
    ]),
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
