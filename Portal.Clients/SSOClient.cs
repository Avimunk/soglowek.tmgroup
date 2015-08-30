using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;


namespace Portal.Clients
{
    public class SSOClient
    {
        public SSOClient()
        {

        }

        private static class Methods
        {
            public static T ldap_get_value<T>(PropertyValueCollection property)
            {
                object value = null;
                foreach (object tmpValue in property) value = tmpValue;
                return (T)value;
            }

            public static string ldap_get_domainname(DirectoryEntry entry)
            {
                if (entry == null || entry.Parent == null) return null;
                using (DirectoryEntry parent = entry.Parent)
                {
                    if (ldap_get_value<string>(parent.Properties["objectClass"]) == "domainDNS")
                        return ldap_get_value<string>(parent.Properties["dc"]);
                    else
                        return ldap_get_domainname(parent);
                }
            }
        }

        public string GetCurrentLoginName()
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.Current;

            if (user != null)
            {
                return user.SamAccountName;
            }

            return null;
        }


        public string GetProperty(string loginName, string propertyName)
        {
            if (!String.IsNullOrEmpty(loginName))
            {
                string[] _properties = new string[] { "objectClass", "distinguishedName", "samAccountName", "userPrincipalName", "displayName", "mail", "title", "company", "thumbnailPhoto", "useraccountcontrol" };
                string account = loginName;

                using (DirectoryEntry ldap = new DirectoryEntry())
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(ldap))
                    {
                        searcher.PropertiesToLoad.AddRange(_properties);
                        if (account.Contains('@')) searcher.Filter = "(userPrincipalName=" + account + ")";
                        else searcher.Filter = "(samAccountName=" + account + ")";
                        var user = searcher.FindOne().GetDirectoryEntry();

                        if (!String.IsNullOrEmpty(propertyName))
                        {
                            return Methods.ldap_get_value<string>(user.Properties[propertyName]);
                        }
                    }
                }
            }

            return null;
        }
    }
}
