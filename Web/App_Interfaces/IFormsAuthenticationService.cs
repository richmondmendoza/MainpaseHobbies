using Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.App_Interfaces
{
    public interface IFormsAuthenticationService
    {
        void SetAuthCookie(AuthenticatedUserDto user, bool createPersistentCookie);
        void SetUserSessionKey();
        AuthenticatedUserDto Identity();
        void SignOut();
    }
}