import {inject, Injectable} from '@angular/core';
import {Actions, createEffect, ofType} from '@ngrx/effects';
import * as GamesActions from './games.actions';
import {catchError, map, mergeMap, of, switchMap} from 'rxjs';
import {GameService} from '../../shared/services/gameService/game-service';

@Injectable()
export class GamesEffects {
  private actions$ = inject(Actions);
  private gameService = inject(GameService);

  loadGames$ = createEffect(() => {

    return this.actions$.pipe(
      ofType(GamesActions.loadGames),
      switchMap(action =>
        this.gameService.getGames(null, action.page, action.pageSize).pipe(
          map(games => GamesActions.loadGamesSuccess({ games })),
          catchError(error => of(GamesActions.loadGamesFailure({ error })))
        )
      )
    );
  });

  loadFilteredGames$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamesActions.loadFilteredGames),
      mergeMap(action =>
        this.gameService.getGames(action.filters, action.page, action.pageSize).pipe(
          map(games => GamesActions.loadGamesSuccess({ games })),
          catchError(error => of(GamesActions.loadGamesFailure({ error })))
        )
      )
    ));
}

