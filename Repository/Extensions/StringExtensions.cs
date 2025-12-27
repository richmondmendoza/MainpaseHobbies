using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions
{
    public static class StringExtensions
    {
        public static string TransformToCardSymbols(this string text)
        {
            string[] classes = new string[]
        {
            "{0}", "{1}", "{2}", "{2B}", "{2G}", "{2R}", "{2U}", "{2W}", "{3}", "{4}", "{5}", "{6}",
            "{7}", "{8}", "{9}", "{10}", "{11}","{12}","{13}","{14}","{15}","{16}","{17}","{18}",
            "{19}","{20}","{100}","{1000000}","{A}","{B}","{B/G}","{B/G/P}","{B/P}","{B/R}","{B/R/P}",
            "{C}","{C/B}","{C/G}","{CHAOS}","{C/P}","{C/R}","{C/U}","{C/W}","{D}","{E}","{G}","{G/P}",
            "{G/U}","{G/U/P}","{G/W}","{G/W/P}","{H}","{HALF}","{H/R}","{H/W}","{INFINITY}","{L}","{P}",
            "{P/W}","{Q}","{R}","{R/G}","{R/G/P}","{R/P}","{R/W}","{R/W/P}","{S}","{T}","{T/K}","{U}","{U/B}",
            "{U/B/P}","{U/P}","{U/R}","{U/R/P}","{W}","{W/B}","{W/B/P}","{W/P}","{W/U}","{W/U/P}","{X}","{Y}","{Z}"
        };

            foreach (var c in classes)
            {
                text = text.Replace(c, $"<span class='card-symbol card-symbol-{c.Replace("/", "").Trim('{', '}').ToUpper()}'></span>");
            }

            return text;
        }
    }
}
