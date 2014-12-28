using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HipchatApiV2.Requests;
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

        public static HttpContent EncodeMultipartRelatedHipchatRequest(this ShareFileWithRoomRequest request)
        {
            var fi = new FileInfo(request.File);
            var fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);

            var multiPartContent = new MultipartContent("related");

            if (!string.IsNullOrEmpty(request.Message))
            {
                var stringContent = new StringContent("{" + "message".EncodeJson() + ": " + request.Message.EncodeJson() + "}", 
                    Encoding.UTF8, "application/json");
                stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    Name = "metadata",
                };
                multiPartContent.Add(stringContent);
            }

            var byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            byteArrayContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Name = "file",
                FileName = Path.GetFileName(fileName)
            };
            multiPartContent.Add(byteArrayContent);
            return multiPartContent;
        }

        public static string AddHipchatAuthentication(this string endpoint, string authToken = null)
        {
            authToken = authToken ?? HipchatApiConfig.AuthToken;
            return endpoint.AddQueryParam("auth_token", authToken);
        }

        public static HttpStatusCode? PostHttpContentToUrl(this string url, HttpContent httpContent)
        {
            HttpStatusCode? result = null;

            var httpClient = new HttpClient();
            httpClient.PostAsync(url, httpContent).
                ContinueWith(t =>
                { 
                    if (t.Status == TaskStatus.RanToCompletion)
                        result = t.Result.StatusCode;
                }).Wait();
            return result;           
        }

    }
}