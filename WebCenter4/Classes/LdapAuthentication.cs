using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace WebCenter4.Classes
{
    public class LdapAuthentication
    {
        private string _path;
        private string _filterAttribute;

        public LdapAuthentication(string path)
        {
            _path = path;
        }

        public bool IsAuthenticatedSimple(string domain, string username, string pwd, ref string ADgroup)
        {
            ADgroup = "";
            DirectoryEntry entry = new DirectoryEntry(_path, domain + @"\" + username, pwd, AuthenticationTypes.Secure);
            try
            {
                DirectorySearcher ds = new DirectorySearcher(entry)
                {
                    Filter = "(SAMAccountName=" + username + ")"
                };
                ds.PropertiesToLoad.Add("cn");
                SearchResult resEnt = ds.FindOne();      
                if (resEnt == null)
                    return false;

                _filterAttribute = (string)resEnt.Properties["cn"][0];
                // ADgroup = GetGroups();
                ADgroup = GetGroupsEx(domain, username, pwd);

                return true;
            }
            catch                      
            {
                return false;
            }

            
        }


        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            return true;
        }


        public string GetGroupsEx(string domain, string username, string pwd)
        {
            StringBuilder groupNames = new StringBuilder();
        
            //PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain);
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, username, pwd);
           UserPrincipal up = UserPrincipal.FindByIdentity(pc, username /*UserPrincipal.Current.SamAccountName*/);

            // Get security groups of the user
            PrincipalSearchResult<Principal> groups1 = up.GetAuthorizationGroups();
            // Do some filtering 
            GroupPrincipal[] filteredGroups = (from p in groups1
                                               where p.ContextType == ContextType.Domain
                                               && p.Guid != null
                                               && p is GroupPrincipal
                                               //   && ((GroupPrincipal)p).GroupScope == GroupScope.Local
                                               // && ((GroupPrincipal)p).GroupScope == GroupScope.Universal
                                               select p as GroupPrincipal).ToArray();
            foreach (GroupPrincipal g in filteredGroups)
            {
                groupNames.Append(g.Name);
                groupNames.Append("|");
            }

            return groupNames.ToString();
        }
    


     
        public string GetGroups()
        {

            StringBuilder groupNames = new StringBuilder();
            try
            {

                DirectorySearcher search = new DirectorySearcher(_path);
                search.Filter = "(cn=" + _filterAttribute + ")";


                SearchResult result = search.FindOne();

                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                Global.logging.WriteLog("Error obtaining group names. " + ex.Message);
                return "";
            }
            return groupNames.ToString();
        } 
       
       

    }
}