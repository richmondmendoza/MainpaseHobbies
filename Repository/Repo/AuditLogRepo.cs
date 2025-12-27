using Database.SQL;
using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class AuditLogRepo
    {
        public static void CreateLog(string action, int user, string username, string table, string data)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var logEntry = new AuditLog
                {
                    Action = action,
                    DateCreated = DateTime.Now,
                    CreatedBy = user,
                    CreatedByUsername = username,
                    TableName = table,
                    Data = data,
                    ApprovedBy = 0,
                    ApprovedByUsername = "",
                    DateApproved = null,
                    DateReviewed = null,
                    ReviewedBy = 0,
                    ReviewedByUsername = ""
                };

                context.AuditLogs.Add(logEntry);
                Db.SaveChanges(context);
            }
        }
    }
}
