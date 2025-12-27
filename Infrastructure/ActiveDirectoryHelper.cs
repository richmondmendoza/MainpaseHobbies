using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Infrastructure
{
    public class ActiveDirectoryHelper
    {
        private readonly string _domainPath;
        private readonly string _username;
        private readonly string _password;

        public ActiveDirectoryHelper(string domainPath, string username, string password)
        {
            _domainPath = "LDAP://" + domainPath;
            _username = username;
            _password = password;
        }

        public ActiveDirectoryReturnValue<ActiveDirectoryUser> Authenticate()
        {
            var usernameIndex = _username.LastIndexOf("\\", StringComparison.CurrentCultureIgnoreCase);

            if (usernameIndex == -1) usernameIndex = 0;

            var username = _username.Substring(usernameIndex, _username.Length - usernameIndex);

            var entry = new DirectoryEntry(_domainPath, _username, _password);
            
            var search = new DirectorySearcher(entry) { Filter = "(SAMAccountName=" + username.Replace("\\","") + ")" };

            string message;

            try
            {
                Object obj = entry.NativeObject;

                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");
                search.PropertiesToLoad.Add("givenName");
                search.PropertiesToLoad.Add("sn");
                search.PropertiesToLoad.Add("objectSid");

                var result = search.FindOne();

                if (null == result)
                {
                    return new ActiveDirectoryReturnValue<ActiveDirectoryUser>("Invalid Username");
                }

                var retUser = new ActiveDirectoryUser
                {
                    UserName = (String) result.Properties["samaccountname"][0]
                };

                if (result.Properties.Contains("displayname"))
                {
                    if (result.Properties.Contains("mail"))
                        retUser.Email = (String) result.Properties["mail"][0];

                    if (result.Properties.Contains("displayname"))
                        retUser.DisplayName = (String) result.Properties["displayname"][0];

                    if (result.Properties.Contains("givenName"))
                        retUser.FirstName = (String) result.Properties["givenName"][0];

                    if (result.Properties.Contains("sn"))
                        retUser.LastName = (String) result.Properties["sn"][0];

                    if (result.Properties.Contains("objectSid"))
                    {
                        var byteSid = (byte[]) result.Properties["objectSid"][0];
                        retUser.Sid = BitConverter.ToString(byteSid);
                    }

                    return new ActiveDirectoryReturnValue<ActiveDirectoryUser>("Successfully Authenticated", true, retUser);
                }

                return new ActiveDirectoryReturnValue<ActiveDirectoryUser>("Invalid Account");
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                entry.Close();
                entry.Dispose();
                search.Dispose();
            }

            return new ActiveDirectoryReturnValue<ActiveDirectoryUser>(message);
        }

        public ActiveDirectoryReturnValue<List<ActiveDirectoryUser>> GetUsers()
        {
            var entry = new DirectoryEntry(_domainPath, _username, _password);
            var search = new DirectorySearcher(entry) { Filter = "(&(objectClass=user)(objectCategory=person))" };
            string message;

            try
            {
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");
                search.PropertiesToLoad.Add("givenName");
                search.PropertiesToLoad.Add("sn");
                search.PropertiesToLoad.Add("objectSid");

                var resultCol = search.FindAll();

                var listAdUser = new List<ActiveDirectoryUser>();

                for (var counter = 0; counter < resultCol.Count; counter++)
                {

                    var result = resultCol[counter];

                    DirectoryEntry de = result.GetDirectoryEntry();


                    if (!IsActive(de) ||
                        !result.Properties.Contains("samaccountname") ||
                        !result.Properties.Contains("displayname"))
                    {
                        continue;
                    }

                    var retUser = new ActiveDirectoryUser
                    {
                        UserName = (String) result.Properties["samaccountname"][0],
                        DisplayName = (String) result.Properties["displayname"][0]
                    };

                    if (result.Properties.Contains("mail"))
                        retUser.Email = (String) result.Properties["mail"][0];

                    if (result.Properties.Contains("givenName"))
                        retUser.FirstName = (String) result.Properties["givenName"][0];

                    if (result.Properties.Contains("sn"))
                        retUser.LastName = (String) result.Properties["sn"][0];

                    if (result.Properties.Contains("objectSid"))
                    {
                        var byteSid = (byte[]) result.Properties["objectSid"][0];
                        retUser.Sid = BitConverter.ToString(byteSid);
                    }

                    listAdUser.Add(retUser);
                }

                return new ActiveDirectoryReturnValue<List<ActiveDirectoryUser>>("Success", true, listAdUser.OrderBy(s=>s.DisplayName).ToList());
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                entry.Close();
                entry.Dispose();
                search.Dispose();
            }

            return new ActiveDirectoryReturnValue<List<ActiveDirectoryUser>>(message);
        }

        //public ActiveDirectoryReturnValue<List<ActiveDirectoryUser>> Search(string displayname)
        //{
        //    var users = GetUsers();
            
        //    if (users.Success)
        //    {
        //        var seachResult = users.ReturnData
        //            .Where(s => s.DisplayName.ToLower().Contains(displayname.ToLower()))
        //            .ToList();

        //        return new ActiveDirectoryReturnValue<List<ActiveDirectoryUser>>("Search Successfull", true, seachResult);
        //    }

        //    return new ActiveDirectoryReturnValue<List<ActiveDirectoryUser>>(users.Message);
        //}

        public ActiveDirectoryReturnValue<List<ActiveDirectoryUser>> Search(string username)
        {
            var users = GetUsers();

            if (users.Success)
            {
                var seachResult = users.ReturnData
                    .Where(s => s.UserName.ToLower().Contains(username.ToLower()))
                    .ToList();

                return new ActiveDirectoryReturnValue<List<ActiveDirectoryUser>>("Search Successfull", true, seachResult);
            }

            return new ActiveDirectoryReturnValue<List<ActiveDirectoryUser>>(users.Message);
        }

        private bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

    }
}
