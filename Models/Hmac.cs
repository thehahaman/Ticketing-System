using NuGet.Common;
using System.Security.Cryptography;

namespace Blockbuster_Rental_Software.Models
{
    public class Hmac
    {
        private string key = "8jLlugGgN/FzhUG+Wt9s/+crFg8GTUqOCYi/t88WziOIjDLZsHxmGlpAXP28kcM6eFn9bxOVCQuWP+uAk8H4Pg==";


        public string hmac(string sensitive)
        {
            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));

            return Convert.ToBase64String(hmac.ComputeHash(Convert.FromBase64String( sensitive )));
        }

    }
}
