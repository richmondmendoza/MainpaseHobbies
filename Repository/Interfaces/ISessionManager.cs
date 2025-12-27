using Database.SQL;
using Dto;
using Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ISessionManager
    {
        string CreateUserSession(IMSEntities context, int employeeId);

        string CreateUserSession(int employeeId);

        UserSessionDto Get(int employeeId);

        void DeleteUserSession(int employeeId);

        ReturnValue ValidateSession(int employeeId, string accessToken);
    }
}
