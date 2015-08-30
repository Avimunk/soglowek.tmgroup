using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using System.Web.Mvc;
using Portal.Controllers;
using System.Web.Helpers;

namespace Portal.Models.Pages
{
    [Validator(typeof(PageFormValidator))]
    public class PageFormModel
    {
        public PageFormModel()
        {
            Types = new List<SelectListItem>() {
				
				new SelectListItem{
					Value="1",
					Text="קטגוריה"
				},
				new SelectListItem{
					Value="2",
					Text="עמוד"
				},
				new SelectListItem{
					Value="3",
					Text="לינק"
				},
                new SelectListItem{
					Value="999",
					Text="טלויזיות"
				},
                 new SelectListItem{
					Value="001",
					Text="MP4"
				}
			};
        }

        public long? ParentId { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public bool IsBlank { get; set; }
        public bool IsIFrame { get; set; }
        public int TypeId { get; set; }

        public long Id { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }

        public string Mp4Content { get; set; }

        public string PageTemplate { get; set; }

        public string Iframelink1 { get; set; }
        public string Iframelink2 { get; set; }
        public string Iframelink3 { get; set; }
        public string Iframelink4 { get; set; }
        public string Iframelink5 { get; set; }


        public string Iframelink1DelayTime { get; set; }
        public string Iframelink2DelayTime { get; set; }
        public string Iframelink3DelayTime { get; set; }
        public string Iframelink4DelayTime { get; set; }
        public string Iframelink5DelayTime { get; set; }





        public string Banner { get; set; }
        public IList<SelectListItem> Types { get; set; }


        public string Picture { get; set; }
        public HttpPostedFileBase _uploadPicture;
        public HttpPostedFileBase UploadPicture
        {
            get
            {
                return _uploadPicture;
            }
            set
            {
                _uploadPicture = value;
                if (_uploadPicture != null)
                {
                    var picture = Guid.NewGuid() + ".jpg";
                    var image = new WebImage(_uploadPicture.InputStream);
                    image.Resize(100, 100).Crop(1, 1).Save("~/public/userfiles/pages/" + picture, "image/jpeg");

                    Picture = picture;
                }
            }
        }

        class PageFormValidator : AbstractValidator<PageFormModel>
        {
            public PageFormValidator()
            {
                RuleFor(x => x.Name).NotEmpty();
            }
        }
    }
}