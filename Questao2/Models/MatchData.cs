﻿using Newtonsoft.Json;

namespace Questao2.Models
{
    public class MatchData
    {
        [JsonProperty("team1goals")]
        public string Team1Goals { get; set; }

        [JsonProperty("team2goals")]
        public string Team2Goals { get; set; }
    }
}
