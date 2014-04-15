using ServiceStack;
using ServiceStack.Text;

namespace HipchatApiV2
{
    public static class HttpHelpers
    {
        public static string FormEncodeHipchatRequest<T>(this T request)
        {
            var dictionary = request.ToStringDictionary();
            var formEncodedString = ""; 
            foreach (var kvp in dictionary)
            {
                if (formEncodedString.Length > 0)
                    formEncodedString += "&";
                formEncodedString += kvp.Key.ToLower() + "=" + kvp.Value;
            }
            return formEncodedString;
        }

        public static string AddHipchatAuthentication(this string endpoint, string authToken = null)
        {
            authToken = authToken ?? HipchatApiConfig.AuthToken;
            return endpoint.AddQueryParam("auth_token", authToken);
        }
    }
}