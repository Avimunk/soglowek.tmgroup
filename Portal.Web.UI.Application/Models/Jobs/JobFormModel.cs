﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models.Jobs
{
    public class JobFormModel
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string Content { get; set; }
		public long Id { get; set; }
	}
}