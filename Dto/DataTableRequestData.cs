using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dto
{
    public class DataTableRequestData
    {
        public int Draw { get { return GetDraw(); } }
        public int SortIndex { get { return GetSortIndex(); } }
        public string SortColumnDirection { get { return GetSortColumnDirection(); } }
        public string SortColumn { get { return GetSortColumn(); } }
        public string SortPattern
        {
            get
            {
                var sortColumn = GetSortColumn();
                var columnDirection = GetSortColumnDirection();

                if (!string.IsNullOrEmpty(sortColumn) & !string.IsNullOrEmpty(columnDirection))
                    return string.Concat(sortColumn, "_", columnDirection).ToUpper();

                return null;
            }
        }
        public int Skip { get { return GetSkip(); } }
        public int PageSize { get { return GetPageSize(); } }
        public string Search { get { return GetSearch(); } }

        public string GetCustomQuery(string querystring)
        {
            return Query(querystring);
        }
        internal int GetDraw()
        {
            return Convert.ToInt32(Query("draw") ?? "0");
        }
        internal int GetSortIndex()
        {
            return Convert.ToInt32(Query("order[0][column]") ?? "0");
        }

        internal string GetSortColumnDirection()
        {
            return Convert.ToString(Query("order[0][dir]") ?? "");
        }

        internal string GetSortColumn()
        {
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                return HttpContext.Current.Request.Params.GetValues("columns[" + HttpContext.Current.Request.Params.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            }
            else
            {
                return HttpContext.Current.Request.QueryString.GetValues("columns[" + HttpContext.Current.Request.QueryString.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            }
        }

        internal int GetSkip()
        {
            return Convert.ToInt32(Query("start") ?? "0");
        }

        internal int GetPageSize()
        {
            return Convert.ToInt32(Query("length") ?? "0");
        }

        internal string GetSearch()
        {
            return Convert.ToString(Query("search[value]") ?? "");
        }

        internal string Query(string param)
        {
            try
            {
                if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
                {
                    return HttpContext.Current.Request.Params[param];
                }
                else
                {
                    return HttpContext.Current.Request.QueryString[param];
                }

            }
            catch
            {
                return null;
            }
        }
    }
}
