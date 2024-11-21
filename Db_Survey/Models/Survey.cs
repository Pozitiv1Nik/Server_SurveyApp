using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEntitySurvey.Models;

namespace Db_Survey.Models
{
    public class Survey
    {
        
        public int Id { get; set; }


        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? Date {  get; set; }



        [ForeignKey("User")]
        public int UserId {  get; set; }


        virtual  public List<Question> Questions { get; set; }

    }
}
