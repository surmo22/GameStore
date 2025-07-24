import {Component, OnInit} from '@angular/core';
import {Observable} from 'rxjs';
import { Game } from '../../../../models/games/game.model';
import * as GamesActions from '../../../../store/games/games.actions';
import * as GamesSelectors from '../../../../store/games/games.selectors';
import {Store} from '@ngrx/store';
import {ReactiveFormsModule} from '@angular/forms';
import {AsyncPipe} from '@angular/common';
import {GameResponse} from '../../../../models/games/game-response';
import {GameCard} from '../../components/game-card/game-card';
import {GameFilters} from '../../components/game-filters/game-filters';
import {GameFilter} from '../../../../models/games/game.filter';
import {PaginationComponent} from '../../components/pagination-component/pagination-component';


@Component({
  selector: 'app-games-list',
  imports: [
    ReactiveFormsModule,
    AsyncPipe,
    GameCard,
    GameFilters,
    PaginationComponent
  ],
  templateUrl: './games-list.html',
  styleUrl: './games-list.scss'
})
export class GamesList implements OnInit {
  games$: Observable<GameResponse>;
  loading$: Observable<boolean>;
  error$: Observable<any>;

  currentPage = 1;
  pageSize = 20;
  private filters: GameFilter = {};


  constructor(private store: Store) {
    this.games$ = this.store.select(GamesSelectors.selectAllGames);
    this.loading$ = this.store.select(GamesSelectors.selectGamesLoading);
    this.error$ = this.store.select(GamesSelectors.selectGamesError);
  }

  ngOnInit(): void {
    this.store.dispatch(GamesActions.loadGames({page: this.currentPage, pageSize: this.pageSize}));
  }

  onFilterChange(filters: GameFilter) {
    this.currentPage = 1;
    this.filters = filters;
    this.store.dispatch(GamesActions.loadFilteredGames({ filters, page: this.currentPage, pageSize: this.pageSize }));
  }

  setPage(page: number){
    this.games$.subscribe((response) => {
      if (page >= 1 && (!response || page <= response.totalPages)) {
        this.currentPage = page;

        // Dispatch action with filters, page, and page size
        this.store.dispatch(
          GamesActions.loadFilteredGames({
            page: this.currentPage,
            pageSize: this.pageSize,
            filters: this.filters, // Pass the filters along with the request
          })
        );
      }
    }).unsubscribe();
  }
}

