using Novell.Directory.Ldap;
using OffSite.Abstraction.Interfaces;
using System;

namespace OffSite.Data.Services
{
    public class LdapService : ILdapService
    {
        public void Init()
        {
            // To do
            string ldapHost = "";
            int ldapPort = 389;
            string userDN = "";
            string userPasswd = "";

            // Creating an LdapConnection instance
            LdapConnection ldapConn = new LdapConnection();

            //Connect function will create a socket connection to the server
            ldapConn.Connect(ldapHost, ldapPort);

            //Bind function will bind the user object Credentials to the Server
            ldapConn.Bind(userDN, userPasswd);

            //// Searches in the Marketing container and return all child entries
            //// just below this container i.e.Single level search

            //var test = ldapConn.GetSchemaDN();

            //LdapSearchResults lsc = ldapConn.Search("ou=sales,o=Acme", LdapConnection.SCOPE_SUB, "(objectClass=*)", null, false);
            //while (lsc.hasMore())
            //{
            //    LdapEntry nextEntry = null;
            //    try
            //    {
            //        nextEntry = lsc.next();
            //    }
            //    catch (LdapException e)
            //    {
            //        Console.WriteLine("Error: " + e.LdapErrorMessage);
            //        //Exception is thrown, go for next entry
            //        continue;
            //    }

            //    Console.WriteLine("\n" + nextEntry.DN);

            //    // Get the attribute set of the entry
            //    LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
            //    System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

            //    // Parse through the attribute set to get the attributes and the
            //    //corresponding values
            //    while (ienum.MoveNext())
            //    {
            //        //34 NDK: LDAP Libraries for C#
            //        //novdocx (ENU)01 February 2006

            //        LdapAttribute attribute = (LdapAttribute)ienum.Current;
            //        string attributeName = attribute.Name;
            //        string attributeVal = attribute.StringValue;
            //        Console.WriteLine(attributeName + "value:" + attributeVal);
            //    }
            //}

            //Procced
            //While all the entries are parsed, disconnect
            ldapConn.Disconnect();
        }
    }
}
