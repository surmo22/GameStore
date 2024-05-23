using GameStore.Data;

namespace GameStore.Services.ReccomendationService
{
    public interface IReccomendationService
    {
        Task<IList<Game>> GetReccomendedGames(IList<Genre> userPreferences);
    }
}
