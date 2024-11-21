﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db_Survey.Models;
using Microsoft.EntityFrameworkCore;
using TestEntitySurvey;

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
            throw new NotImplementedException("Object has NULL");
        }
        public async Task AddUserAsync(User user)
        {

            if (user != null)
            {
                await Context.Users.AddAsync(user);
                await Context.SaveChangesAsync();
            }
            throw new NotImplementedException("Object has NULL");


        }

        public bool CheckUser(string login,string password)
        {
            return Context.Users.Any(item => item.Login == login);
  
        }


        public async Task<bool> CheckUserAsync(string login,string password)
        {      
          return await Context.Users.AnyAsync(item => item.Login == login);
        }

        public List<User> GetUsers()
        {
            return Context.Users.ToList();
        }


        public async Task<List<User>> GetUsersAsync()
        {
            return await Context.Users.ToListAsync();
        }

        public async Task<User> GetUsersByLoginAsync(string login)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }


        public User GetUserByLogin(string login) 
        { 
         return Context.Users.FirstOrDefault(u => u.Login == login);
        
        }


        public User GetUserByPassword(string password)
        {
            return Context.Users.FirstOrDefault(u => u.Password == u.Password);

        }

        public async Task<User> GetUserByPasswordAsync(string password)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.Password == u.Password);

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
