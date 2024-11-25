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
           await Context.Results.AddAsync(new Result {SurveyId = surveyId,TextOption = optionText.Trim()});
           await Context.SaveChangesAsync();


        }
        public async Task<List<ResultGroup>> GetCountVotesForResultAsync(int surveyId)
        {
            var groupedResults = await Context.Results
            .Where(v => v.SurveyId == surveyId) 
            .GroupBy(v => v.TextOption)       
            .Select(g => new ResultGroup
             {
              OptionText = g.Key,          
              VoteCount = g.Count()        
            })
             .ToListAsync();

            return groupedResults;

        }

        public class ResultGroup
        {
            public string OptionText { get; set; } 
            public int VoteCount { get; set; }   
        }


    }
}
