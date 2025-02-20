using Questao2.Repositories;

namespace Questao2.Services
{
    public class FootballMatchService
    {
        private readonly IFootballMatchRepository _repository;

        public FootballMatchService(IFootballMatchRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> GetTotalScoredGoals(string team, int year)
        {
            return await _repository.GetTotalScoredGoals(team, year);
        }
    }
}
