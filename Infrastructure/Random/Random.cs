
namespace Infrastructure.Random
{
    public class RandomString
    {
        public static string RandomText(int length)
        {
            int PasswordLength = length;
            string _allowedChars = "ABCDEFGHJKLMNPRSTUVWXYZ";
            System.Random randNum = new TT800();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                randNum.NextDouble();
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                randNum.NextDouble();
            }

            return new string(chars);
        }
        public static string RandomTextNumbers(int length)
        {
            int PasswordLength = length;
            string _allowedChars = "abcdefghjklmnpqrstuvwxyz23456789";
            System.Random randNum = new TT800();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                randNum.NextDouble();
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                randNum.NextDouble();
            }

            return new string(chars);
        }
        public static string RandomNumber(int length)
        {
            int PasswordLength = length;
            string _allowedChars = "123456789";
            System.Random randNum = new TT800();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                randNum.NextDouble();
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                randNum.NextDouble();
            }

            return new string(chars);
        }
    }
}

