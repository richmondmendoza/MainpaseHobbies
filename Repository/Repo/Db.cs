using Database.SQL;
using Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class Db
    {
        public static int SaveChanges(IMSEntities entities, ReturnValue result, string successMessage,
            string dbConcurrencyErrorMessage, string dbUpdateMessage)
        {
            int success = -1;
            try
            {
                success = entities.SaveChanges();
                result.Message = successMessage;
                result.Success = true;

                if (success == 0)
                    result.Message = "Nothing to save.";
            }
            catch (DbUpdateException uError)
            {
                result.Message = (!string.IsNullOrEmpty(dbUpdateMessage) ? dbUpdateMessage + " " : "") + "(DB Update Error): " + uError.Message;
                result.Success = false;
            }
            catch (DBConcurrencyException cError)
            {
                result.Message = (!string.IsNullOrEmpty(dbConcurrencyErrorMessage) ? dbConcurrencyErrorMessage + " " : "") + "(DB Concurrency Error): [" + cError.Message + "]";
                result.Success = false;
            }
            catch (DbException dbEx)
            {
                result.Message = "DB Exception Error: [" + dbEx.Message + "]";
                result.Success = false;
            }
            catch (Exception ex)
            {
                result.Message = "Exception Error: [" + ex.Message + "]";
                result.Success = false;
            }

            result.Data = success;
            return success;
        }

        public static int SaveChanges(IMSEntities entities, ReturnValue result, string successMessage)
        {
            return SaveChanges(entities, result, successMessage, "", "");
        }

        public static void SaveChanges(IMSEntities entities)
        {
            SaveChanges(entities, new ReturnValue(), "", "", "");
        }
    }
}
