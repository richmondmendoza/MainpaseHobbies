using Dto.User;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UserViewModel
    {
        public UserViewModel() { }
        public UserViewModel(UserDto dto)
        {
            if (dto != null)
            {
                Id = dto.Id;
                Firstname = dto.Firstname;
                Middlename = dto.Middlename;
                LastName = dto.LastName;
                Username = dto.Username;
                DateCreated = dto.DateCreated;
                LastLoginDate = dto.LastLoginDate;
                InvalidLoginDate = dto.InvalidLoginDate;
                InvalidLoginCounter = dto.InvalidLoginCounter;
                Role = dto.Role;
                IsDeleted = dto.IsDeleted;
                IsLocked = dto.IsLocked;
            }
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string Firstname { get; set; }
        public string Middlename { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime InvalidLoginDate { get; set; }
        public int InvalidLoginCounter { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }

        public List<string> Roles
        {
            get
            {
                var roles = Role.Split('|').Where(a => !string.IsNullOrEmpty(a));
                return roles.ToList();
            }
        }

        public UserDto ToDto()
        {
            return new UserDto
            {
                Id = this.Id,
                Firstname = this.Firstname,
                Middlename = this.Middlename,
                LastName = this.LastName,
                Username = this.Username,
                DateCreated = this.DateCreated,
                LastLoginDate = this.LastLoginDate,
                InvalidLoginDate = this.InvalidLoginDate,
                InvalidLoginCounter = this.InvalidLoginCounter,
                Role = this.Role,
                IsDeleted = this.IsDeleted,
                IsLocked = this.IsLocked
            };
        }
    }

    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Enter your new password.")]
        public string NewPassword { get; set; }

        [RequiredIf("NewPassword", Operator.NotEqualTo, "", ErrorMessage = "Enter your new password.")]
        [EqualTo("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}