using System.Security.Cryptography;
using System.Text;

namespace LundBot.Application.Common.Security.Hashing
{
    public static class HashUtils
    {
        public static byte[] HashString(string input)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(input));
        }
    }
}
