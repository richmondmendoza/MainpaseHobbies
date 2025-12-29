using Database.SQL;
using Dto;
using Dto.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.User
{
    public class CustomerRepo
    {
        public CustomerDetailDto ToDto(CustomerDetail item)
        {
            if (item == null) return null;

            return new CustomerDetailDto()
            {
                Id = item.Id,
                UserId = item.UserId,
                DateCreated = item.DateCreated,
                Firstname = item.Firstname,
                Lastname = item.Lastname,
                Email = item.Email,
                Mobile = item.Mobile,
                Address1 = item.Address1,
                Address2 = item.Address2,
                Country = item.Country,
                Postal = item.Postal,
                ShippingAddress1 = item.ShippingAddress1,
                ShippingAddress2 = item.ShippingAddress2,
                ShippingCountry = item.ShippingCountry,
                IsDeleted = item.IsDeleted,
                ShippingPostal = item.ShippingPostal,
            };
        }

        public CustomerDetailDto GetDetail(string email)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.CustomerDetails.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
                return ToDto(item);
            }
        }

        public CustomerDetailDto GetByUserId(int userId)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.CustomerDetails.FirstOrDefault(a => a.UserId == userId);
                return ToDto(item);
            }
        }

        public ReturnValue Add(int userId, string email)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var existing = context.CustomerDetails.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
                if (existing == null)
                {
                    var newCustomer = new CustomerDetail()
                    {
                        UserId = userId,
                        DateCreated = DateTime.Now,
                        Firstname = "",
                        Lastname = "",
                        Email = email,
                        Mobile = "",
                        Address1 = "",
                        Address2 = "",
                        Country = "",
                        Postal = "",
                        ShippingAddress1 = "",
                        ShippingAddress2 = "",
                        ShippingCountry = "",
                        IsDeleted = false,
                        ShippingPostal = "",
                    };

                    context.CustomerDetails.Add(newCustomer);
                }

                Db.SaveChanges(context, result, "Customer added.");
            }

            return result;
        }

        public ReturnValue Add(CustomerDetailDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var existing = context.CustomerDetails.FirstOrDefault(a => a.Email.ToLower() == dto.Email.ToLower());
                if (existing != null)
                {
                    existing.UserId = dto.UserId;
                    existing.DateCreated = dto.DateCreated;
                    existing.Firstname = dto.Firstname;
                    existing.Lastname = dto.Lastname;
                    existing.Email = dto.Email;
                    existing.Mobile = dto.Mobile;
                    existing.Address1 = dto.Address1;
                    existing.Address2 = dto.Address2;
                    existing.Country = dto.Country;
                    existing.Postal = dto.Postal;
                    existing.ShippingAddress1 = dto.ShippingAddress1;
                    existing.ShippingAddress2 = dto.ShippingAddress2;
                    existing.ShippingCountry = dto.ShippingCountry;
                    existing.IsDeleted = dto.IsDeleted;
                    existing.ShippingPostal = dto.ShippingPostal;

                    Db.SaveChanges(context, result, "Customer added.");
                    result.Data = existing.Id;
                }
                else
                {
                    var newCustomer = new CustomerDetail()
                    {
                        UserId = dto.UserId,
                        DateCreated = dto.DateCreated,
                        Firstname = dto.Firstname,
                        Lastname = dto.Lastname,
                        Email = dto.Email,
                        Mobile = dto.Mobile,
                        Address1 = dto.Address1,
                        Address2 = dto.Address2,
                        Country = dto.Country,
                        Postal = dto.Postal,
                        ShippingAddress1 = dto.ShippingAddress1,
                        ShippingAddress2 = dto.ShippingAddress2,
                        ShippingCountry = dto.ShippingCountry,
                        IsDeleted = dto.IsDeleted,
                        ShippingPostal = dto.ShippingPostal,
                    };

                    context.CustomerDetails.Add(newCustomer);

                    Db.SaveChanges(context, result, "Customer added.");
                    result.Data = result.Success ? newCustomer.Id : 0;
                }
            }

            return result;
        }

        public void SubscribeToNewsLetter(string email)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var existing = context.NewsLetters.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
                if (existing == null)
                {
                    context.NewsLetters.Add(new NewsLetter() { Email = email });
                    Db.SaveChanges(context);
                }
            }


        }
    }
}
