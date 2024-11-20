using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db_Survey.Models;

namespace Db_Survey
{
    internal class SurveyDbManagerUser
    {
        public SurveyDbContext Context { get; set; }

        public SurveyDbManagerUser(SurveyDbContext context)
        {
            Context = context;
        }

        public void AddUser(User user)
        {
            
            if(user != null)
            {
                Context.Users.Add(user);
                Context.SaveChanges();
            }
           
        }


        public List<User> GetUsers()
        {
            return Context.Users.ToList();
        }

        public User GetUserByLogin(string login) 
        { 
         return Context.Users.FirstOrDefault(u => u.Login == login);
        
        }


        public User GetUserByPassword(string password)
        {
            return Context.Users.FirstOrDefault(u => u.Password == u.Password);

        }
        public List<User> GetUsersCustomer()
        {

            return Context.Users.Where(u => u.RoleId == 1).ToList();

        }
        public List<User> GetUsersAdmin()
        {
            return Context.Users.Where(u => u.RoleId == 2).ToList();

        }







    }
}
