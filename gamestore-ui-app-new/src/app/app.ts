import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {SharedModule} from './shared/shared-module';
import {AuthModule} from './features/auth/auth-module';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SharedModule, AuthModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'gamestore-ui-app-new';
}
