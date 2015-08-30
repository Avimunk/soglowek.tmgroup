using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Portal.Clients;

namespace Portal.Clients.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SSOClient ssoClient = new SSOClient();

            string ssoLoginName = ssoClient.GetCurrentLoginName();
            if (!String.IsNullOrEmpty(ssoLoginName))
            {
                string ssoPropertyName = "st";

                if (!String.IsNullOrEmpty(ssoPropertyName))
                {
                    string ssoPropertyValue = ssoClient.GetProperty(ssoLoginName, ssoPropertyName);

                    if (!String.IsNullOrEmpty(ssoPropertyValue))
                    {
                        Int64 localId = Int64.Parse(ssoPropertyValue);

                        if (localId > 0)
                        {
                            Console.WriteLine("ID: " + localId);
                            Console.ReadLine();
                        }
                    }
                }
            }
        }
    }
}
