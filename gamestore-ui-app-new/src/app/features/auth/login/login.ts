import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {SharedModule} from '../../../shared/shared-module';
import {AuthService} from '../../../shared/services/authService/auth-service';
import {LoginRequest} from '../../../models/auth/login-request';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    SharedModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginPage implements OnInit{
  form!: FormGroup;

  constructor(private fb: FormBuilder,
              private authService: AuthService) {}

  ngOnInit() {
    this.form = this.fb.group({
      email: ['', [Validators.required, /*Validators.email*/]],
      password: ['', Validators.required]
    });
  }

  onInternalLogin() {
    if (this.form.valid) {
      const loginRequest: LoginRequest = {
        login: this.form.get('email')?.value,
        password: this.form.get('password')?.value,
        internalAuth: true
      };

      this.authService.login(loginRequest).subscribe();
    }
  }

  onExternalLogin() {
    if (this.form.valid) {
      const loginRequest: LoginRequest = {
        login: this.form.get('email')?.value,
        password: this.form.get('password')?.value,
        internalAuth: false
      };

      this.authService.login(loginRequest).subscribe();
    }
  }
}
