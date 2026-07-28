using System.Security.Cryptography;
using System.Text;

namespace LundBot.Utils
{
    public static class HashUtils
    {
        public static byte[] HashString(string input)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(input));
        }
    }
}
