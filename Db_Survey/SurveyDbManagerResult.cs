using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEntitySurvey;
using Db_Survey.Models;
using Microsoft.EntityFrameworkCore;

namespace Db_Survey
{
    public class SurveyDbManagerResult
    {
        public SurveyDbContext Context;

        public SurveyDbManagerResult(SurveyDbContext context)
        {
            Context = context;

        }

        public async Task AddResult(string optionText,int surveyId)
        {
            Context.Results.AddAsync(new Result {SurveyId = surveyId,TextOption = optionText.Trim()});
            Context.SaveChangesAsync();


        }
        public async Task<int> GetCountVotesForResultAsync(int surveyId,string optionText)
        {
            var voteCount = await Context.Results
           .Where(v => v.SurveyId == surveyId && v.TextOption == optionText).CountAsync();

            return voteCount;

        }




    }
}
