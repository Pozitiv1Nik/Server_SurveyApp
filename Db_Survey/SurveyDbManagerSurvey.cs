using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEntitySurvey.Models;
using Db_Survey.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace TestEntitySurvey
{
    internal class SurveyDbManagerSurvey
    {

        public SurveyDbContext Context { get; set; }

        public SurveyDbManagerSurvey(SurveyDbContext context) 
        {

        Context = context;
        
        }

        public Survey GetSurveyById(int surveyId)
        {
            return Context.Surveys.FirstOrDefault(u => u.Id == surveyId);
        }

        public async Task<Survey>GetSurveyByIdAsync(int surveyId)
        {
            return await Context.Surveys.FirstOrDefaultAsync(u => u.Id == surveyId);
        }

        public Survey GetSurveyByTitle(string title)
        {
            return Context.Surveys.FirstOrDefault(item => item.Title == title);

        }

        public async Task<Survey>GetSurveyByTitleAsync(string title)
        {
            return await Context.Surveys.FirstOrDefaultAsync(item => item.Title == title);
          
        }

        public List<Survey> GetSurveys()
        {
            return Context.Surveys.ToList();
        }

        public async Task<List<Survey>> GetSurveysAsync()
        {
            return await Context.Surveys.ToListAsync();
        }




        public void DeleteSurvey(int id)
        {
            Context.Surveys.Remove(Context.Surveys.FirstOrDefault(item => item.Id == id));
            Context.SaveChanges();
        }

        public async Task DeleteSurveyAsync(int id)
        {
            Context.Surveys.Remove(await Context.Surveys.FirstOrDefaultAsync(item => item.Id == id));
            await Context.SaveChangesAsync();
        }




        public void AddSurvey(Survey survey)
       {
            Context.Surveys.Add(survey);
            Context.SaveChanges();

       }

       public async Task AddSurveyAsync(Survey survey)
       {
            await Context.AddAsync(survey);
            await Context.SaveChangesAsync();
               
       }







    }
}
