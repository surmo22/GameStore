// src/app/store/games/games.reducer.ts
import { createReducer, on } from '@ngrx/store';
import * as GamesActions from './games.actions';
import {GameResponse} from '../../models/games/game-response';


export interface GamesState {
  games: GameResponse;
  loading: boolean;
  error: any;

}

export const initialState: GamesState = {
  games: null!,
  loading: false,
  error: null
};

export const gamesFeatureKey = 'games';

export const gamesReducer = createReducer(
  initialState,
  on(GamesActions.loadGames, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(GamesActions.loadGamesSuccess, (state, { games }) => ({
    ...state,
    games,
    loading: false
  })),
  on(GamesActions.loadGamesFailure, (state, { error }) => ({
    ...state,
    error,
    loading: false
  }))
);
