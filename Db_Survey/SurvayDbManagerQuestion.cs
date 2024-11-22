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
    internal class SurvayDbManagerQuestion
    {
        public SurveyDbContext Context { get; set; }

        public SurvayDbManagerQuestion(SurveyDbContext context) {

            Context = context;

        }


        public List<Question> GetQuestionsBySurveyId(int surveyId)
        {

            return Context.Question.Where(q => q.SurveyId == surveyId).ToList();

        }



        public void RemoveQuestionById(int id)
        {

            Context.Question.Remove(Context.Question.FirstOrDefault(item => item.Id == id));
            Context.SaveChanges();
        }

        public void RemoveAllInSurveyById(int surveyId)
        {
           Context.Question.RemoveRange(Context.Question.Where(item => item.SurveyId == surveyId));

        }

        public async Task AddQuestionAsync(Question question)
        {
            await Context.Question.AddAsync(question);
            await Context.SaveChangesAsync();
        }

        public async Task AddQuestionAsync(List<Question> questions)
        {
            await Context.Question.AddRangeAsync(questions);
            await Context.SaveChangesAsync();

        }



    }
}
