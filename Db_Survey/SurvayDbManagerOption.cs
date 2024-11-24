using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db_Survey.Models;
using Microsoft.EntityFrameworkCore;
using TestEntitySurvey;
using TestEntitySurvey.Models;


namespace Db_Survey
{
    public class SurveyDbManagerOption
    {
        public SurveyDbContext Context { get; set; }

        public SurveyDbManagerOption(SurveyDbContext context) {

            Context = context;

        }


        public List<Option> GetOptionsBySurveyId(int surveyId)
        {

            return Context.Options.Where(q => q.SurveyId == surveyId).ToList();

        }



        public void RemoveOptionById(int id)
        {

            Context.Options.Remove(Context.Options.FirstOrDefault(item => item.Id == id));
            Context.SaveChanges();
        }

        public void RemoveAllInSurveyById(int surveyId)
        {
           Context.Options.RemoveRange(Context.Options.Where(item => item.SurveyId == surveyId));

        }

        public async Task AddOptionAsync(Option question)
        {
            await Context.Options.AddAsync(question);
            await Context.SaveChangesAsync();
        }

        public async Task AddOptionAsync(List<Option> questions)
        {
            await Context.Options.AddRangeAsync(questions);
            await Context.SaveChangesAsync();

        }



    }
}
