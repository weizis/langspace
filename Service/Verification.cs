using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LangSpace.Service
{
    class Verification
    {
        public static string GetSHA512Hash(string input)
        {
            SHA512CryptoServiceProvider SHA512Hasher = new SHA512CryptoServiceProvider();
            byte[] data = SHA512Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
        public static bool VerifySHA512Hash(string input, string hash)
        {
            string hashOfInput = GetSHA512Hash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
