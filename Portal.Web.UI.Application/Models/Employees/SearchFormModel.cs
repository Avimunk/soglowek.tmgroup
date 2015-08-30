using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;

namespace Portal.Models.Employees
{
	public class SearchFormModel
	{
        public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public long? JobTitleId { get; set; }
		public long? DepartmentId { get; set; }
	
		
		public IList<Employee> Employees { get; set; }
	}
}