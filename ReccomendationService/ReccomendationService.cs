using Accord.MachineLearning;
using Accord.Math.Distances;
using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReccomendationService
{
    class ReccomendationService
    {
        private readonly ApplicationDbContext context;
        public ReccomendationService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async void Main(string[] args)
        {
            // Sample data of games
            List<Game> games = await context.Games.ToListAsync();


            // List of all possible genres
            List<Genre> allGenres = await context.Genres.ToListAsync();

            // Transform games into numerical feature vectors
            double[][] features = games.Select(game =>
            {
                // One-hot encode the genres
                double[] genreFeatures = allGenres.Select(genre => game.Genres.Contains(genre) ? 1.0 : 0.0).ToArray();
                return genreFeatures;
            }).ToArray();

            // Define the number of clusters (k)
            int k = 3;

            // Set a random seed for reproducibility
            Accord.Math.Random.Generator.Seed = 42;

            // Create and initialize the KMeans algorithm
            KMeans kmeans = new KMeans(k)
            {
                Distance = new SquareEuclidean()
            };

            // Compute the clusters
            KMeansClusterCollection clusters = kmeans.Learn(features);

            // Get the assignments of data points to clusters
            int[] assignments = clusters.Decide(features);

            // User's preferences (e.g., genres they like)
            List<Genre> userPreferences = new List<Genre> { allGenres[0], allGenres[1] }; // Example user preferences

            // Find the cluster that best matches the user's preferences
            int userCluster = FindBestMatchingCluster(assignments, games, userPreferences, allGenres);

            // Recommend games from the cluster that matches the user's preferences
            List<Game> recommendedGames = RecommendGamesFromCluster(games, assignments, userCluster);

            // Display recommended games
            Console.WriteLine("Recommended Games:");
            foreach (var game in recommendedGames)
            {
                Console.WriteLine(game.Title);
            }
        }

        static int FindBestMatchingCluster(int[] assignments, List<Game> games, List<Genre> userPreferences, List<Genre> allGenres)
        {
            // One-hot encode the user preferences
            double[] userPreferencesVector = allGenres.Select(genre => userPreferences.Contains(genre) ? 1.0 : 0.0).ToArray();

            // Calculate the similarity of the user preferences vector to each cluster centroid
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

            // Find the cluster with the highest similarity score
            return clusterSimilarities.OrderByDescending(x => x.Value).First().Key;
        }

        static List<Game> RecommendGamesFromCluster(List<Game> games, int[] assignments, int clusterIndex)
        {
            // Get games from the specified cluster
            var recommendedGames = games.Where(game => assignments[game.Id - 1] == clusterIndex).ToList();
            return recommendedGames;
        }
    }

}
