import {Game} from './game.model';

export interface GameResponse {
  games: Game[];
  totalPages: number;
  currentPage: number
}
