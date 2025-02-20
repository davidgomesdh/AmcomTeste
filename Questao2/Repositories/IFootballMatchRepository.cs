namespace Questao2.Repositories
{
    public interface IFootballMatchRepository
    {
        Task<int> GetTotalScoredGoals(string team, int year);
    }
}
