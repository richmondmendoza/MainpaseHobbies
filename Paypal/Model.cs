using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paypal
{
    public class PaypalModel
    {
        public static string SecretId { get { return "EEnlG3pACVh2VLSFeQfeOUpD_kpH-DyGRBhNSOXhDuGZLWgM8_2Ft4_eYQIGU9J8Y_pPM6kQO4i6AgH9"; } }
        public static string ClientId { get { return "AZySPKvjh8eQS46sAJ3C0D_PwxPqVclQ_pk7k1teat96fp47iE-18CQXV5GalVXb-oS-1w6c4Qi-Xikw"; } }
    }

    public class SandboxModel
    {
        public static string Url { get { return "https://sandbox.paypal.com"; } }
        public static string Email { get { return "sb-cqemi48723384@business.example.com"; } }
        public static string Password { get { return "|nUB?8=y"; } }
        public static string Region { get { return "PH"; } }
    }

    public class LiveModel
    {
        public static string Url { get { return ""; } }
        public static string Email { get { return ""; } }
        public static string Password { get { return ""; } }
        public static string Region { get { return "PH"; } }
    }

}
