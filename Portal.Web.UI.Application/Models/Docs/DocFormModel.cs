using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace Portal.Models.Docs
{
    public class DocFormModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }
        public string Downloaded { get; set; }
        public string Submited { get; set; }
        public string NotSubmited { get; set; }
        public string Units { get; set; }
        public long Id { get; set; }
        public bool isAvailable2All { get; set; }
    

       
    }
}