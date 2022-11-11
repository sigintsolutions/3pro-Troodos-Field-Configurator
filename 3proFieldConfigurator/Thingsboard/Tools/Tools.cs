using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thingsboard.Tools
{
    public static class Tools
    {
        public static string DecodeJWT(string jwtToken)
        {
            var base64Url = jwtToken.Split('.')[1]; //jwt token has 3 sections. we are interested in the middle one
                                                    //JWT uses base64url (RFC 4648 §5), so using only atob (which uses base64) isn't enough.
            var base64 = base64Url.Replace('-', '+').Replace('_', '/');
            string jsonPayload = Base64Decode((string)base64);
            return jsonPayload;
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
