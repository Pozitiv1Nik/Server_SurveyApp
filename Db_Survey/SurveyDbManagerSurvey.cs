using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEntitySurvey.Models;
using Db_Survey.Models;

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



        public List<Survey> GetSurveys()
        {
            return Context.Surveys.ToList();
        }

       public void DeleteSurvey(int id)
       {
            Context.Surveys.Remove(Context.Surveys.FirstOrDefault(item => item.Id == id));
            Context.SaveChanges();
       }

       public void AddSurvey(Survey survey)
       {
            Context.Surveys.Add(survey);
            Context.SaveChanges();

       }





    }
}
