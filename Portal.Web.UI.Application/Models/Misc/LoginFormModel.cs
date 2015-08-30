using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using System.Xml.Linq;
using Portal.Entities;
using FluentValidation.Attributes;
using FluentValidation.Validators;
using FluentValidation.Results;
using System.Web.Security;
using System.Net;

namespace Portal.Models.Misc
{
	[Validator(typeof(LoginFormValidator))]
	public class LoginFormModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class LoginFormValidator : AbstractValidator<LoginFormModel>
	{
        public LoginFormValidator()
        {
            RuleFor(x => x.Username).Cascade(CascadeMode.StopOnFirstFailure)
              .Must(CheckUsername).WithMessage("שדה אימייל חובה")
              .Must(CheckPassword).WithMessage("שדה סיסמא חובה")
              .Must(CheckUsernamePassword).WithMessage("אימייל או סיסמא שגויים");
        }

        private bool CheckUsernamePassword(LoginFormModel model, string username)
        {
            using (var session = NHibernateHelper.OpenSession()){
                var emp = session.QueryOver<Employee>().Where(x => x.Email == model.Username && x.Password == model.Password && x.IsActive == true).SingleOrDefault();
                var adm = session.QueryOver<Employee>().Where(x => x.Username == model.Username).SingleOrDefault();
                if (emp != null || adm != null)
                {
                    return true;
                }
        }
            return false;
        }

        private bool CheckUsername(string username)
        {
            return !string.IsNullOrEmpty(username);
        }

        private bool CheckPassword(string password)
        {

            return !string.IsNullOrEmpty(password);
        }
	}
}