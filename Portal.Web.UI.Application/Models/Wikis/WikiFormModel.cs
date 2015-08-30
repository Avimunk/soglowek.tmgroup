using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using System.Web.Mvc;
using Portal.Controllers;
using System.IO;
using Portal.Entities;

namespace Portal.Models.Wikis
{
    public class WikiFormModel:ApplicationController
    {
      

       
        public long Id { get; set; }
      
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string LetterEn { get; set; }
        public string Letter { get; set; }
        public string AddedBy { get; set; }
        public string Content { get; set; }
    }
}