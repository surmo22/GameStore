export const environment = {
  apiBaseUrl: 'https://localhost:7091/',
  getImageUrl: (gameKey: string) => `https://localhost:7091/games/${gameKey}/image`
};
