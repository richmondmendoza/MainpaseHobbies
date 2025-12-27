using Database.SQL;
using Dto.BaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Settings
{
    public class SystemInfoRepo
    {
        public static SystemInfoDto GetSystemInfo()
        {
            using (var context = new IMSEntities())
            {
                var info = context.System_WebsiteInfo.FirstOrDefault();
                if (info != null)
                {
                    return new SystemInfoDto
                    {
                        Id = info.Id,
                        Name = info.Name,
                        LongName = info.LongName,
                        Website = info.Website,
                        Email = info.Email,
                        ContactNumber1 = info.ContactNumber1,
                        ContactNumber2 = info.ContactNumber2,
                        ContactEmail1 = info.ContactEmail1,
                        ContactEmail2 = info.ContactEmail2,
                        Address1 = info.Address1,
                        Address2 = info.Address2,
                        WorkingHour = info.WorkingHour,
                        FB = info.FB,
                        Insta = info.Insta,
                        Twitter = info.Twitter,
                        LinkeIn = info.LinkeIn
                    };
                }
            }

            return null;
        }
    }
}
