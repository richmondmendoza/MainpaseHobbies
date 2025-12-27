namespace Infrastructure
{
    public class ActiveDirectoryReturnValue
    {
        public ActiveDirectoryReturnValue(string message, bool sucess = false, object returnParam = null)
        {
            Success = sucess;
            Message = message;
            ReturnParam = returnParam;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object ReturnParam { get; set; }
    }


    public class ActiveDirectoryReturnValue<T> where T : class, new()
    {
        public ActiveDirectoryReturnValue(string message, bool success = false, T tRet = null)
        {
            Success = success;
            Message = message;
            ReturnData = tRet;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public T ReturnData { get; set; }
    }
}