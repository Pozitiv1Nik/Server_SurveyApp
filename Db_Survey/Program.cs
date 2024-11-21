using TestEntitySurvey;
using Db_Survey.Models;

namespace Db_Survey
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new SurveyDbContextFactory();
           
            using (var db = factory.CreateDbContext(args))
            {

                //Role role = new Role();
                //{
                //    role.UserType = Roles.Customer;
                //}
                //db.Roles.Add(role);
                //db.SaveChanges();
                //User user = new User();
                //{

                //    user.Password = "password";
                //    user.Login = "Login";
                //    user.RoleId = 1;


                //}
                //db.Users.Add(user);
                //db.SaveChanges();
                //Survey survey = new Survey();
                //{
                //    survey.Title = "SomeTitle";
                //    survey.Questions = new List<Question>();
                //    survey.Date = DateTime.Now;
                //    survey.Description = "Description";
                //    survey.UserId = 1;


                //}
                //db.Surveys.Add(survey);       
                //db.SaveChanges();
                //foreach (var item in db.Users)
                //{
                //    Console.WriteLine(item.Password);

                //}

            }






        }
    }
}
