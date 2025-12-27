using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public IEnumerable<string> Results { get; set; } = new List<string>();
    }
}