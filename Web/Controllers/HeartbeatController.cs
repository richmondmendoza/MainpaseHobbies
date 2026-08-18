using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HeartbeatController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return Content("OK", "text/plain");
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Status()
        {
            var result = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                database = CheckDatabase()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string CheckDatabase()
        {
            var text = "";
            try
            {
                var efConnStr = ConfigurationManager.ConnectionStrings["FinLiqEntities"].ConnectionString;
                var builder = new EntityConnectionStringBuilder(efConnStr);
                using (var conn = new SqlConnection(builder.ProviderConnectionString))
                {
                    conn.Open();
                }
                text += "DB Healthy\r\n\r\n";
            }
            catch
            {
                text += "DB Unhealthy\r\n\r\n";
            }

            return text;
        }

    }
}