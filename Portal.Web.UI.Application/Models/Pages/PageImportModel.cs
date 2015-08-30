using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using System.Web.Mvc;
using Portal.Controllers;
using System.Web.Helpers;

namespace Portal.Models.Pages
{
    [Validator(typeof(PageImportValidator))]
    public class PageImportModel
    {
        public PageImportModel()
        {
        }

        public long Id { get; set; }
        public string FolderUrl { get; set; }
        public string TemplateUrl { get; set; }
        public string TemplateItemUrl { get; set; }


        class PageImportValidator : AbstractValidator<PageImportModel>
        {
            public PageImportValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.FolderUrl).NotEmpty();
                RuleFor(x => x.TemplateUrl).NotEmpty();
                RuleFor(x => x.TemplateItemUrl).NotEmpty();
            }
        }
    }
}