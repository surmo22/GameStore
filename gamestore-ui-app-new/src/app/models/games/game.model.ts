export interface Game {
  id: string;
  key: string;
  name: string;
  description: string | null;
  price: number;
  unitInStock: number;
  discount: number;
}
