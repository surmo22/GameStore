import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginPage} from './login/login';

const routes: Routes = [
  {
    path: 'login',
    component: LoginPage
  }
  // Add other auth-related routes here
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
