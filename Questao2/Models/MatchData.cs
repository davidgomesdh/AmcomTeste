using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
