using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEntitySurvey.Models
{
    internal class Question
    {
        public int Id { get; set; }

        [Required]
        public string QuestionText {  get; set; }



        [ForeignKey("Survey")]
        public int  SurveyId { get; set; }
      
       virtual public List<Answer> Answers {  get; set; }



    }
}
