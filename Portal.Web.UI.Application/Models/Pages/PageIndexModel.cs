using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using System.Web.Mvc;

namespace Portal.Models.Pages
{
	public class PageIndexModel
	{
		public PageIndexModel() {
			Sections = new List<SelectListItem>() {
				
			new SelectListItem{
					Value="1",
					Text="תפריט ראשי"
				},
			
				new SelectListItem{
					Value="3",
					Text="תפריט תחתון"
				}
			};
			Title = "תפריט";
		}

		private int _sectionId = 1;
		public int SectionId {
			get {
				return _sectionId;
			}
			set {
				_sectionId = value;
			}
		}
		public IList<SelectListItem> Sections { get; set; }
		public IList<Page> Pages { get; set; }
		public long? ParentId { get; set; }
		public long? PrevParentId { get; set; }
		public string Title { get; set; }
	}
}