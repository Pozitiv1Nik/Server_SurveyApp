﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEntitySurvey.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        public string AnswerText { get; set; }

        [ForeignKey("Question")]
        public int QuestionId {  get; set; }

        virtual public Question Question { get; set; }


    }
}
