using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment.Helpers
{
    /// <summary>
    /// MD5 calculations
    /// </summary>
    public class MD5Helper
    {
        /// <summary>
        /// Calculates MD5 hash from the provided string input.
        /// </summary>
        /// <param name="input">String to hash</param>
        public static string GetMD5StringSum(string input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
