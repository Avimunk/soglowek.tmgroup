using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using FluentValidation;

namespace Portal.Models.FlashBanners
{
    [Validator(typeof(FlashBannerFormValidator))]
    public class FlashBannerFormModel
    {
        public void InitMembers()
        {
            Types = new List<SelectListItem>() {
				
				new SelectListItem{
					Value="1",
					Text="תמונה"
				},
				new SelectListItem{
					Value="2",
					Text="youtube"
				}
				,
				new SelectListItem{
					Value="3",
					Text="swf"
				}
			};
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Content2 { get; set; }
        private string _banner = string.Empty;
        public string Banner
        {
            get
            {
                return _banner;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string[] banners = value.Split('|').Where(x => x.Trim().Length > 0).Distinct().ToArray();
                    _banner = string.Join("|", banners);
                }
            }
        }
        public string Url { get; set; }
        public string YouTubeCode { get; set; }
      
        public int TypeId { get; set; }
        public IList<SelectListItem> Types { get; set; }
    }

    class FlashBannerFormValidator : AbstractValidator<FlashBannerFormModel>
    {
        public FlashBannerFormValidator()
        {

        }


    }
}