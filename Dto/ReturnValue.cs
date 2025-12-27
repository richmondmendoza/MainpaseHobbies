using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class ReturnValue
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ReturnValue(string message = "", bool success = true, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
