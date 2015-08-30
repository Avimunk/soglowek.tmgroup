using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using Portal.Entities;

namespace Portal.Controllers
{
	public class NotificationsController : MailerBase
	{
		//
		// GET: /Mailers/

		public EmailResult Bday(Employee manager, Employee employee) {
			ViewBag.Employee = employee;
			Subject = "היום יום הולדת ל" + employee.FullName;
			To.Add(manager.Email);
			return Email("Bday", manager);
		}

		public EmailResult Forum(Employee employee) {
			Subject = "קיבלת הודעה אישית בפורטל";
			To.Add(employee.Email);
			return Email("Forum", employee);
		}

		public EmailResult Message(Employee employee) {
			Subject = "קיבלת הודעה אישית בפורטל";
			To.Add(employee.Email);
			return Email("Message", employee);
		}

	}
}
