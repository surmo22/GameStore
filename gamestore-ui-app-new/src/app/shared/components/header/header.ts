import { Component } from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [
    RouterLinkActive,
    RouterLink,
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  cartItemCount: number = 0;
}
