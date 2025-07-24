import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {Game} from '../../../models/games/game.model';
import {HttpClient, HttpParams} from '@angular/common/http';
import {GameResponse} from '../../../models/games/game-response';
import {GameFilter} from '../../../models/games/game.filter';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = 'https://localhost:7091/games';

  constructor(private http: HttpClient) { }

  getGames(filters: GameFilter | null, page: number, pageSize: number): Observable<GameResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.priceRange) {
        if (filters.priceRange.min != null) {
          params = params.append('minPrice', filters.priceRange.min.toString());
        }
        if (filters.priceRange.max != null) {
          params = params.append('maxPrice', filters.priceRange.max.toString());
        }
      }

      Object.entries(filters).forEach(([key, value]) => {
        if (!value || key === 'priceRange') return;

        if (Array.isArray(value)) {
          value.forEach(item => {
            params = params.append(key, item);
          });
        } else {
          params = params.append(key, value.toString());
        }
      });
    }

    return this.http.get<GameResponse>(this.apiUrl, { params });
  }


  getGameById(id: string): Observable<Game> {
    return this.http.get<Game>(`${this.apiUrl}/${id}`);
  }

  createGame(game: Omit<Game, 'id'>): Observable<Game> {
    return this.http.post<Game>(this.apiUrl, game);
  }

  updateGame(game: Game): Observable<Game> {
    return this.http.put<Game>(`${this.apiUrl}/${game.id}`, game);
  }

  deleteGame(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
