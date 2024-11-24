using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db_Survey.Models
{
    public class Result
    {
        public int Id { get; set; }

        [ForeignKey("Option")]
        public int OptionId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } 

        public virtual Option Option { get; set; }
        public virtual User User { get; set; }


    }
}
