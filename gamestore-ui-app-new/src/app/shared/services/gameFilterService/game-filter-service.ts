import { Injectable } from '@angular/core';
import {environment} from '../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {SortOption} from '../../../models/games/sort.option';
import {Observable} from 'rxjs';
import {Publisher} from '../../../models/games/publisher';
import {ReleaseDateOption} from '../../../models/games/release-date.option';
import {Genre} from '../../../models/games/genre';
import {Platform} from '../../../models/games/platform';

@Injectable({
  providedIn: 'root'
})
export class GameFiltersService {
  private baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getSortOptions(): Observable<SortOption[]> {
    return this.http.get<SortOption[]>(`${this.baseUrl}games/sorting-options`);
  }

  getPublishers(): Observable<Publisher[]> {
    return this.http.get<Publisher[]>(`${this.baseUrl}publishers`);
  }

  getReleaseDateOptions(): Observable<ReleaseDateOption[]> {
    return this.http.get<ReleaseDateOption[]>(`${this.baseUrl}games/publish-date-options`);
  }

  getGenres(): Observable<Genre[]> {
    return this.http.get<Genre[]>(`${this.baseUrl}genres`);
  }

  getPlatforms(): Observable<Platform[]> {
    return this.http.get<Platform[]>(`${this.baseUrl}platforms`);
  }
}

