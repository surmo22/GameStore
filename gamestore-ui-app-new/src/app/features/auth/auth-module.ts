import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing-module';
import {LoginPage} from './login/login';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    AuthRoutingModule,
    LoginPage
  ],
  exports: [LoginPage]
})
export class AuthModule { }
