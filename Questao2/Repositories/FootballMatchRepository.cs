using Newtonsoft.Json;

namespace Questao2.Repositories
{
    public class FootballMatchRepository : IFootballMatchRepository
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<int> GetTotalScoredGoals(string team, int year)
        {
            int totalGoals = 0;


            totalGoals += await GetGoalsFromTeam(team, year, "team1");


            totalGoals += await GetGoalsFromTeam(team, year, "team2");

            return totalGoals;
        }

        private async Task<int> GetGoalsFromTeam(string team, int year, string teamPosition)
        {
            int totalGoals = 0;
            int page = 1;

            try
            {
                while (true)
                {

                    string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&page={page}&{teamPosition}={team}";


                    var response = await _httpClient.GetStringAsync(url);


                    var matchData = JsonConvert.DeserializeObject<ApiResponse>(response);


                    if (matchData == null || matchData.Data == null || matchData.Data.Count == 0)
                    {
                        break;
                    }


                    foreach (var match in matchData.Data)
                    {
                        if (teamPosition == "team1")
                        {
                            totalGoals += Convert.ToInt32(match.Team1Goals);
                        }
                        else if (teamPosition == "team2")
                        {
                            totalGoals += Convert.ToInt32(match.Team2Goals);
                        }
                    }

                    if (page >= matchData.TotalPages)
                    {
                        break;
                    }

                    page++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar os dados: {ex.Message}");
            }

            return totalGoals;
        }
    }

    public class ApiResponse
    {
        public int Page { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        public List<Match> Data { get; set; }
    }

    public class Match
    {
        public string Competition { get; set; }
        public string Year { get; set; }
        public string Round { get; set; }

        public string Team1 { get; set; }
        public string Team1Goals { get; set; }

        public string Team2 { get; set; }
        public string Team2Goals { get; set; }
    }
}
