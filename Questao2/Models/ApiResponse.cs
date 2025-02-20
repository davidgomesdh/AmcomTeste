using Newtonsoft.Json;

namespace Questao2.Models
{
    public class ApiResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("data")]
        public List<MatchData> Data { get; set; }
    }
}
