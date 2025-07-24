import { createAction, props } from '@ngrx/store';
import { Game } from '../../models/games/game.model';
import {GameResponse} from '../../models/games/game-response';
import {GameFilter} from '../../models/games/game.filter';

export const loadGames = createAction(
  '[Games] Load Games',
  props<{ page: number; pageSize: number }>()
);

export const loadGamesSuccess = createAction(
  '[Games] Load Games Success',
  props<{ games: GameResponse }>()
);

export const loadGamesFailure = createAction(
  '[Games] Load Games Failure',
  props<{ error: any }>()
);

export const loadFilteredGames = createAction(
  '[Games] Load Filtered Games',
  props<{ filters: GameFilter; page: number; pageSize: number }>()
);
