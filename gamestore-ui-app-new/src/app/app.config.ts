import {ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {provideStore} from '@ngrx/store';
import {gamesFeatureKey, gamesReducer} from './store/games/games.reducer';
import {provideEffects} from '@ngrx/effects';
import {GamesEffects} from './store/games/games.effects';
import {provideStoreDevtools} from '@ngrx/store-devtools';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {authInterceptor} from './features/auth/authInterceptor/auth-interceptor';
import {GameService} from './shared/services/gameService/game-service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideStore({[gamesFeatureKey]: gamesReducer}),
    provideEffects([GamesEffects]),
    provideStoreDevtools({ maxAge: 25, logOnly: false }),
    provideHttpClient(withInterceptors([authInterceptor])),
    GameService
  ]
};
