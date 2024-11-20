using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db_Survey.Models
{
    internal class User
    {

        public int Id { get; set; }

        
        [Required]
        
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [ForeignKey("Role")]
        public int RoleId {  get; set; }

       virtual public List<Survey>Surveys { get; set; }






    }
}
