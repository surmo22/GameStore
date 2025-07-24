import {PriceRange} from './price.range';

export interface GameFilter {
  sort?: string;
  priceRange?: PriceRange;
  publisher?: string[];
  datePublishing?: string;
  genres?: string[];
  platforms?: string[];
}
