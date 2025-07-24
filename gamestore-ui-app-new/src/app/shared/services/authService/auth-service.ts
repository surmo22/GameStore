import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { LoginRequest } from '../../../models/auth/login-request';
import {environment} from '../../../../environments/environment';
import {BehaviorSubject, Observable} from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'auth_token';
  private isAuthenticated$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) { }

  login(loginRequest: LoginRequest) {
    return this.http.post<{ token: string }>(`${environment.apiBaseUrl}Users/login`, {model: loginRequest})
      .pipe(
        tap(token => {
          localStorage.setItem(this.tokenKey, token.token);
          this.isAuthenticated$.next(true);
        })
      )
  }

  getToken() : string | null {
    return localStorage.getItem(this.tokenKey);
  }

  logout(): void{
    localStorage.removeItem(this.tokenKey);
    this.isAuthenticated$.next(false);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  getAuthStatus(): Observable<boolean> {
    return this.isAuthenticated$.asObservable();
  }

}
