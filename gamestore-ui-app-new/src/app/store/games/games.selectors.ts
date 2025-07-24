// src/app/store/games/games.selectors.ts
import { createFeatureSelector, createSelector } from '@ngrx/store';
import {gamesFeatureKey, GamesState} from './games.reducer';

export const selectGamesState = createFeatureSelector<GamesState>(gamesFeatureKey);

export const selectAllGames = createSelector(
  selectGamesState,
  (state: GamesState) => state.games
);

export const selectGamesLoading = createSelector(
  selectGamesState,
  (state: GamesState) => state.loading
);

export const selectGamesError = createSelector(
  selectGamesState,
  (state: GamesState) => state.error
);

export const selectTotalPages = createSelector(
  selectGamesState,
  (state: GamesState) => state.games.totalPages
);


