import { Component, Input } from '@angular/core';
import {Game} from '../../../../models/games/game.model';
import {environment} from '../../../../../environments/environment';
import {DecimalPipe} from '@angular/common';

@Component({
  selector: 'app-game-card',
  imports: [
    DecimalPipe
  ],
  templateUrl: './game-card.html',
  styleUrl: './game-card.scss'
})
export class GameCard {
  @Input() game!: Game;
  protected readonly environment = environment;

  getImageUrl(gameKey: string): string {
    return this.environment.getImageUrl(gameKey);
  }

  onImageError($event: ErrorEvent) {
    const img = $event.target as HTMLImageElement;
    img.src = "assets/images/header-image.jpg";
  }
}
