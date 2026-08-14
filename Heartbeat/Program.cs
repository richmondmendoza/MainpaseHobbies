using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var url = System.Configuration.ConfigurationSettings.AppSettings["heartbeatURL"];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "html/text";
            WebResponse response = request.GetResponse();
            var responseString = new
            StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseString);
        }
    }
}
