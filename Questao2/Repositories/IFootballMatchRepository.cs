using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Questao2.Models;

namespace Questao2.Repositories
{
    public interface IFootballMatchRepository
    {
        Task<int> GetTotalScoredGoals(string team, int year);
    }
}
