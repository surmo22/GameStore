import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {GameFiltersService} from '../../../../shared/services/gameFilterService/game-filter-service';
import {PriceRange} from '../../../../models/games/price.range';
import {Platform} from '../../../../models/games/platform';
import {Genre} from '../../../../models/games/genre';
import {ReleaseDateOption} from '../../../../models/games/release-date.option';
import {Publisher} from '../../../../models/games/publisher';
import {SortOption} from '../../../../models/games/sort.option';
import {GameFilter} from '../../../../models/games/game.filter';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-game-filters',
  imports: [
    FormsModule
  ],
  templateUrl: './game-filters.html',
  styleUrl: './game-filters.scss'
})
export class GameFilters implements OnInit {
  @Output() filtersChanged = new EventEmitter<GameFilter>();

  sortOptions: SortOption[] = [];
  publishers: Publisher[] = [];
  releaseDateOptions: ReleaseDateOption[] = [];
  genres: Genre[] = [];
  platforms: Platform[] = [];

  selectedFilters: GameFilter = {};
  priceRange: PriceRange = { min: 0, max: 10000 };

  loading: boolean = false;
  error: boolean = false;

  constructor(private filtersService: GameFiltersService) {}

  ngOnInit() {
    this.loadFilterOptions();
  }

  private loadFilterOptions() {
    this.filtersService.getSortOptions().subscribe(
      options => this.sortOptions = options
    );

    this.filtersService.getPublishers().subscribe(
      publishers => this.publishers = publishers
    );

    this.filtersService.getReleaseDateOptions().subscribe(
      options => this.releaseDateOptions = options
    );

    this.filtersService.getGenres().subscribe(
      genres => this.genres = genres
    );

    this.filtersService.getPlatforms().subscribe(
      platforms => this.platforms = platforms
    );
  }

  onFilterChange() {
    this.filtersChanged.emit(this.selectedFilters);
  }

  onPriceRangeChange() {
    this.selectedFilters = {
      ...this.selectedFilters,
      priceRange: { ...this.priceRange }
    };
    this.onFilterChange();
  }

  onPublisherChange(publisherId: string, event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;
    const updatedPublishers = this.selectedFilters.publisher
      ? [...this.selectedFilters.publisher] // clone existing array
      : []; // or start with empty array

    if (isChecked) {
      updatedPublishers.push(publisherId);
    }
    else {
      // Remove unchecked publisher
      const index = updatedPublishers.indexOf(publisherId);
      if (index > -1) {
        updatedPublishers.splice(index, 1);
      }
    }
    // Create a *new* filters object immutably
    this.selectedFilters = {
      ...this.selectedFilters,
      publisher: updatedPublishers
    };

    this.onFilterChange();
  }


  onGenreChange(genreId: string, event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;

    const updatedGenres = this.selectedFilters.genres
      ? [...this.selectedFilters.genres]
      : [];

    if (isChecked) {
      updatedGenres.push(genreId);
    }
    else {
      const index = updatedGenres.indexOf(genreId);
      if (index > -1) {
        updatedGenres.splice(index, 1);
      }
    }

    this.selectedFilters = {
      ...this.selectedFilters,
      genres: updatedGenres
    };

    this.onFilterChange();
  }

  onPlatformChange(platformId: string, event: Event): void {
    const isChecked = (event.target as HTMLInputElement).checked;

    const updatedPlatforms = new Set(this.selectedFilters.platforms || []);

    if (isChecked) {
      updatedPlatforms.add(platformId);
    }
    else {
      updatedPlatforms.delete(platformId);
    }

    this.selectedFilters = {
      ...this.selectedFilters,
      platforms: Array.from(updatedPlatforms)
    };

    this.onFilterChange();
  }

  onSortChange(newSort: string) {
    this.selectedFilters = {
      ...this.selectedFilters,
      sort: newSort
    };
    this.onFilterChange();
  }

  onReleaseDateChange(option: ReleaseDateOption) {
    this.selectedFilters = {
      ...this.selectedFilters,
      datePublishing: option
    };
    this.onFilterChange();
  }
}

