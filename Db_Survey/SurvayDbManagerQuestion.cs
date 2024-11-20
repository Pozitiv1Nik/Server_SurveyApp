using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db_Survey.Models;


namespace Db_Survey
{
    internal class SurvayDbManagerQuestion
    {
        public SurveyDbContext Context { get; set; }

        public SurvayDbManagerQuestion(SurveyDbContext context) {

            Context = context;

        }


        public List<Question> GetQuestionBySurveyId(int surveyId)
        {

            return Context.Question.Where(q => q.SurveyId == surveyId).ToList();

        }



        public void RemoveSurvay(int id)
        {

            Context.Question.Remove(Context.Question.FirstOrDefault(item => item.Id == id));
            Context.SaveChanges();
        }




   
    }
}
