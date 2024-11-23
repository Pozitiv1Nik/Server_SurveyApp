using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db_Survey.Models;
using Microsoft.EntityFrameworkCore;
using TestEntitySurvey;
using TestEntitySurvey.Models;

namespace Db_Survey
{
    public class SurveyDbManagerUser
    {
        private SurveyDbContext Context { get; set; }

        public SurveyDbManagerUser(SurveyDbContext context)
        {
            Context = context;
        }

        public void AddUser(User user)
        {
            try
            {
                if (user != null)
                {
                    Context.Users.Add(user);
                    Context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public async Task<bool> IsAdminAsync(string login,string password)
        {
           return await Context.Users.AnyAsync(item => item.Login == login && item.RoleId == 2);
            
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                if (user != null)
                {
                    await Context.Users.AddAsync(user);
                    await Context.SaveChangesAsync();
                }

            }
            catch (Exception ex) {
                throw new NotImplementedException("Object has NULL");

            }
          
        }

        public bool CheckUser(string login,string password)
        {
            return Context.Users.Any(item => item.Login == login && item.Password == password);
  
        }


        public async Task<bool> CheckUserAsync(string login,string password)
        {
            try
            {
                return await Context.Users.AnyAsync(item => item.Login == login && item.Password == password);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false; 
            }
        }

        public List<User> GetUsers()
        {
            return Context.Users.ToList();
        }


        public async Task<List<User>> GetUsersAsync()
        {
            return await Context.Users.ToListAsync();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public User GetUserByLogin(string login) 
        { 
         return Context.Users.FirstOrDefault(u => u.Login == login);
        
        }


        public User GetUserByPassword(string password)
        {
            return Context.Users.FirstOrDefault(u => u.Password == password);

        }

        public async Task<User> GetUserByPasswordAsync(string password)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.Password == password);

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
