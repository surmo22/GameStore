using Accord.MachineLearning;
using Accord.Math.Distances;
using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Services.ReccomendationService
{
    public class ReccomendationService : IReccomendationService
    {
        private readonly ApplicationDbContext context;
        public int K { get; set; } = 7;

        public ReccomendationService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IList<Game>> GetReccomendedGamesAsync(IList<Genre> userPreferences)
        {
            var games = await context.Games.Include(g => g.Genres).ToListAsync();

            List<Genre> allGenres = await context.Genres.ToListAsync();

            double[][] features = games.Select(game =>
            {
                double[] genreFeatures = allGenres.Select(genre => game.Genres.Contains(genre) ? 1.0 : 0.0).ToArray();
                return genreFeatures;
            }).ToArray();

            KMeans kmeans = new KMeans(K)
            {
                Distance = new SquareEuclidean()
            };

            KMeansClusterCollection clusters = kmeans.Learn(features);

            int[] assignments = clusters.Decide(features);

            var gameIdToIndex = games.Select((game, index) => new { game.Id, Index = index })
                                     .ToDictionary(x => x.Id, x => x.Index);

            int userCluster = FindBestMatchingCluster(assignments, games, userPreferences, allGenres);
            IList<Game> recommendedGames = RecommendGamesFromCluster(games, assignments, userCluster, gameIdToIndex);
            return recommendedGames;
        }

        private int FindBestMatchingCluster(int[] assignments, IList<Game> games, IList<Genre> userPreferences, IList<Genre> allGenres)
        {
            double[] userPreferencesVector = allGenres.Select(genre => userPreferences.Contains(genre) ? 1.0 : 0.0).ToArray();
            var clusterSimilarities = new Dictionary<int, double>();
            for (int i = 0; i < games.Count; i++)
            {
                int clusterIndex = assignments[i];
                double similarity = userPreferencesVector.Zip(allGenres.Select(genre => games[i].Genres.Contains(genre) ? 1.0 : 0.0), (u, g) => u * g).Sum();
                if (!clusterSimilarities.ContainsKey(clusterIndex))
                    clusterSimilarities[clusterIndex] = similarity;
                else
                    clusterSimilarities[clusterIndex] += similarity;
            }

            return clusterSimilarities.OrderByDescending(x => x.Value).First().Key;
        }

        private IList<Game> RecommendGamesFromCluster(IList<Game> games, int[] assignments, int clusterIndex, Dictionary<int, int> gameIdToIndex)
        {
            var recommendedGames = games.Where(game => gameIdToIndex.ContainsKey(game.Id) && assignments[gameIdToIndex[game.Id]] == clusterIndex).ToList();
            return recommendedGames;
        }
    }
}
