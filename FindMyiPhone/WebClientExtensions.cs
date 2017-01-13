using System.Net;

namespace FindMyiPhone
{
    public static class WebClientExtensions
    {
        public static string PostDataToWebsite(this WebClient client, string url, string postData)
        {
            client.Encoding = System.Text.Encoding.UTF8;
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            return client.UploadString(url, "POST", postData); ;
        }
    }
}
