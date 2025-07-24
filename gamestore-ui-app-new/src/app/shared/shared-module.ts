import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ButtonComponent} from './components/button/button';
import {Footer} from './components/footer/footer';
import {Header} from './components/header/header';
import {Social} from './components/social/social';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ButtonComponent,
    Footer,
    Header,
    Social,
  ],
  exports: [
    ButtonComponent,
    Footer,
    Header,
    Social,
  ],
})
export class SharedModule { }
