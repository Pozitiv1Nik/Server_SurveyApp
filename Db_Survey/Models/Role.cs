using Db_Survey.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestEntitySurvey.Models
{

    public enum Roles { 
    
        Customer = 1,
        Admin = 2 
    }

    public class Role
    {   
        public int Id { get; set; }

        [Required]
        public Roles UserType { get; set; }


        virtual public List<User> Users { get; set; }


    }
}
