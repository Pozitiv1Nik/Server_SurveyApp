using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db_Survey.Models
{
    public class Option
    {
        public int Id { get; set; }

        [Required]
        public string OptionText { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        public virtual Survey Survey { get; set; }


    }
}
