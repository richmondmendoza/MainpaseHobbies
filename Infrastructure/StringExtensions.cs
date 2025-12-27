using System.Text;

namespace Infrastructure
{
    public static class StringExtensions
    {
        public static string ToReadableText(this string s)
        {
            if (string.IsNullOrEmpty(s) || 2 > s.Length)
            {
                return s;
            }

            var sb = new StringBuilder();
            var ca = s.ToCharArray();
            sb.Append(ca[0]);
            for (int i = 1; i < ca.Length - 1; i++)
            {
                char c = ca[i];
                if (char.IsUpper(c) && (char.IsLower(ca[i + 1]) || char.IsLower(ca[i - 1])))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            sb.Append(ca[ca.Length - 1]);
            return sb.ToString();
        }

    }
}
