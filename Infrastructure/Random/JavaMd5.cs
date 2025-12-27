using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Random
{
    public class JavaMd5
    {
        public static string GetHash(string username, string password)
        {
            string hash = getMd5Hash(username, password);
            return hash;
        }
        public static string getMd5Hash(string input1, string input2)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input1 + input2));
            //data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input2));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


    }

}
