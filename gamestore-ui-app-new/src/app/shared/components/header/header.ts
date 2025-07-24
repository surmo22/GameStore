import {Component, OnDestroy} from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {AuthService} from '../../services/authService/auth-service';
import {Subject, takeUntil} from 'rxjs';

@Component({
  selector: 'app-header',
  imports: [
    RouterLinkActive,
    RouterLink,
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header implements OnDestroy {
  cartItemCount: number = 0;
  public isLoggedIn: boolean = false;
  private destroy$ = new Subject<void>();

  constructor(private authService: AuthService){
    this.authService.getAuthStatus()
      .pipe(
        takeUntil(this.destroy$)
      )
      .subscribe(isAuthenticated => {
        this.isLoggedIn = isAuthenticated;
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  logout() {
    this.authService.logout();
  }
}
