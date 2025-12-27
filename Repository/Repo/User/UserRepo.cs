using Database.SQL;
using Dto;
using Dto.Enums;
using Dto.User;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.User
{
    public class UserRepo
    {
        public UserRepo() { }

        public UserDto ToDto(Database.SQL.User user)
        {
            if (user == null) return null;

            return new UserDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                LastName = user.LastName,
                DateCreated = user.DateCreated,
                InvalidLoginCounter = user.InvalidLoginCounter,
                InvalidLoginDate = user.InvalidLoginDate ?? DateTime.MinValue,
                IsDeleted = user.IsDeleted,
                IsLocked = user.IsLocked,
                LastLoginDate = user.LastLoginDate ?? DateTime.MinValue,
                Middlename = user.Middlename,
                Password = "",
                PasswordKey = "",
                Role = user.Role,
                Username = user.Username,
            };
        }

        public ReturnValue Authenticate(string username, string password)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var user = context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

                if (user == null)
                {
                    result.Success = false;
                    result.Message = "User not found.";
                    return result;
                }

                var encryptPassword = Fletcher.Encrypt(password, user.PasswordKey);
                if (user.Password != encryptPassword)
                {
                    result.Success = false;
                    result.Message = "Username and password do not match.";

                    user.InvalidLoginDate = DateTime.Now;
                    user.InvalidLoginCounter++;

                    if (user.InvalidLoginCounter >= 5)
                    {
                        user.IsLocked = true;
                        result.Message = "Your account has been locked due to multiple invalid login attempts.";
                    }

                    Db.SaveChanges(context);
                }
                else
                {
                    user.InvalidLoginCounter = 0;
                    user.LastLoginDate = DateTime.Now;
                    user.InvalidLoginDate = null;
                    user.IsLocked = false;

                    var roles = user.Role.Split('|').Select(r => r.Trim()).ToList();
                    AuthenticatedUserDto authUser = new AuthenticatedUserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FirstName = user.Firstname,
                        LastName = user.LastName,
                        HasAdminAccess = roles.Any(a => a == UserRoleEnum.PortalAdmin.ToString() || a == UserRoleEnum.PortalUser.ToString()),
                        HasCustomerAccess = roles.Any(a => a == UserRoleEnum.Customer.ToString()),
                        Role = user.Role,
                    };

                    Db.SaveChanges(context, result, "Authentication successful.");

                    if (result.Success)
                    {
                        var token = new SessionManagerRepo().CreateUserSession(context, user.Id);
                        authUser.AccessToken = token;
                        result.Data = authUser;
                    }
                }

            }

            return result;
        }

        public IEnumerable<UserDto> GetList()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var records = context.Users.Where(a => !a.IsDeleted).ToList();
                return records.Select(a => ToDto(a));
            }
        }

        public UserDto Get(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Users.Where(a => a.Id == id).FirstOrDefault();
                return ToDto(record);
            }
        }

        public ReturnValue Create(UserDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var user = context.Users.Where(a => a.Username == dto.Username).FirstOrDefault();

                dto.PasswordKey = Guid.NewGuid().ToString();
                var encryptedPassword = Fletcher.Decrypt(dto.Password, dto.PasswordKey);

                if (user != null)
                {
                    if (!user.IsDeleted)
                        return new ReturnValue("Username already exists.");

                    user.IsDeleted = false;
                    user.Firstname = dto.Firstname;
                    user.LastName = dto.LastName;
                    user.DateCreated = DateTime.Now;
                    user.InvalidLoginCounter = 0;
                    user.InvalidLoginDate = null;
                    user.IsLocked = false;
                    user.LastLoginDate = null;
                    user.Middlename = dto.Middlename;
                    user.PasswordKey = dto.PasswordKey;
                    user.Password = encryptedPassword;
                    user.Role = dto.Role;

                    var sessions = context.User_Session.Where(a => a.UserId == user.Id);
                    context.User_Session.RemoveRange(sessions);
                }
                else
                {

                    user = new Database.SQL.User()
                    {
                        Firstname = dto.Firstname,
                        LastName = dto.LastName,
                        DateCreated = DateTime.Now,
                        InvalidLoginCounter = 0,
                        InvalidLoginDate = null,
                        IsDeleted = dto.IsDeleted,
                        IsLocked = dto.IsLocked,
                        LastLoginDate = null,
                        Middlename = dto.Middlename,
                        Password = encryptedPassword,
                        PasswordKey = dto.PasswordKey,
                        Role = dto.Role,
                        Username = dto.Username,
                    };

                    context.Users.Add(user);
                }

                Db.SaveChanges(context, result, "Successfully Added.");
                result.Data = ToDto(user);
            }

            return result;
        }

        public ReturnValue Update(UserDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Users.Where(a => a.Id == dto.Id).FirstOrDefault();

                if (record == null)
                    return new ReturnValue("User details not found.");

                record.IsDeleted = dto.IsDeleted;
                record.Firstname = dto.Firstname;
                record.LastName = dto.LastName;
                record.IsLocked = dto.IsLocked;
                record.Middlename = dto.Middlename;
                record.Role = dto.Role;

                Db.SaveChanges(context, result, "Successfully Updated.");
                result.Data = ToDto(record);
            }

            return result;
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Users.Where(a => a.Id == id).FirstOrDefault();

                if (record == null)
                    return new ReturnValue("User details not found.");

                record.IsDeleted = true;

                result.Data = ToDto(record);
                Db.SaveChanges(context, result, "Successfully Deleted.");
            }

            return result;
        }

        public ReturnValue ChangePassword(int userid, string password)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Users.Where(a => a.Id == userid).FirstOrDefault();

                if (record == null)
                    return new ReturnValue("User details not found.");

                var passKey = Guid.NewGuid().ToString();
                var encPass = Fletcher.Encrypt(password, passKey);

                record.Password = encPass;
                record.PasswordKey = passKey;

                Db.SaveChanges(context, result, "Password changed successfully.");
                result.Data = ToDto(record);
            }

            return result;
        }

        public ReturnValue ClearSession(int userId)
        {
            var result = new ReturnValue("Session cleared.", true);

            using (IMSEntities context = new IMSEntities())
            {
                var user = context.Users.FirstOrDefault(a => a.Id == userId);
                var records = context.User_Session.Where(a => a.UserId == userId);

                if (records.Any())
                {
                    context.User_Session.RemoveRange(records);
                    Db.SaveChanges(context, result, $"Session cleared for user [{(user?.Firstname ?? "")} {(user?.LastName ?? "")}].");
                }
            }

            return result;
        }

    }
}
