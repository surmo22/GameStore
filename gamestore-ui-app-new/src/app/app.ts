import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {SharedModule} from './shared/shared-module';
import {AuthModule} from './features/auth/auth-module';
import {StoreModule} from '@ngrx/store';
import {EffectsModule} from '@ngrx/effects';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    SharedModule,
    AuthModule,
    StoreModule,
    EffectsModule
  ],

  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'gamestore-ui';
}
