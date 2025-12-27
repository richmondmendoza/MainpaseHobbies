using Database.SQL;
using Dto;
using Dto.BaseSettings;
using Dto.User;
using Repository.Interfaces;
using Repository.Repo.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.User
{
    public class SessionManagerRepo : ISessionManager
    {
        public string CreateUserSession(IMSEntities context, int userId)
        {
            var accessToken = Guid.NewGuid().ToString("D");

            var existingToken = context.User_Session.Where(s => s.UserId == userId);

            if (existingToken.Any())
            {
                existingToken.ToList().ForEach(s => s.IsTerminated = true);
            }

            var user = context.Users.FirstOrDefault(a => a.Id == userId);
            var token = new User_Session()
            {
                UserId = userId,
                Date = DateTime.Now,
                LastDateOfActivity = DateTime.Now,
                Token = accessToken,
                IsTerminated = false,
                IPAddress = Utilities.IPAddress(),
                FullName = user != null ? $"{user.Firstname} {user.LastName}" : string.Empty,
            };

            context.User_Session.Add(token);

            if (context.SaveChanges() > 0)
            {
                return token.Token;
            }

            return string.Empty;
        }

        public string CreateUserSession(int employeeId)
        {
            using (IMSEntities context = new IMSEntities())
            {
                return CreateUserSession(context, employeeId);
            }
        }

        public void DeleteUserSession(int employeeId)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var sessions = context.User_Session.Where(a => a.UserId == employeeId);

                foreach (var session in sessions)
                {
                    context.User_Session.Remove(session);
                }

                context.SaveChanges();
            }
        }

        public UserSessionDto Get(int userId)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.User_Session.OrderByDescending(a => a.Id).FirstOrDefault(a => a.UserId == userId);

                if (record == null)
                    return null;

                return new UserSessionDto()
                {
                    Id = record.Id,
                    IPAddress = record.IPAddress,
                    UserId = record.UserId,
                    IsTerminated = record.IsTerminated,
                    LastDateOfActivity = record.LastDateOfActivity,
                    Date = record.Date,
                    Token = record.Token,
                    FullName = record.FullName,
                };
            }
        }

        public ReturnValue ValidateSession(int employeeId, string accessToken)
        {
            var result = new ReturnValue("Invalid Session!");

            using (IMSEntities context = new IMSEntities())
            {
                var ip = Utilities.IPAddress();
                var existingToken = context.User_Session.FirstOrDefault(s =>
                    s.UserId == employeeId &&
                    s.IPAddress == ip &&
                    s.IsTerminated == false);

                if (existingToken != null & accessToken == (existingToken?.Token ?? ""))
                {
                    existingToken.LastDateOfActivity = DateTime.Now;
                    context.SaveChanges();

                    result.Success = true;
                    result.Message = "Session is valid.";
                }
            }

            return result;
        }
    }
}
