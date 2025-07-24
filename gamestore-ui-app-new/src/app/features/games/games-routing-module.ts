import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {GamesList} from './pages/games-list/games-list';

const routes: Routes = [
  {
    path: '',
    component: GamesList
  }
  // Add other game-related routes here
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GamesRoutingModule { }
