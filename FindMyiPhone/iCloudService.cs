using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FindMyiPhone
{
    public class iCloudService
    {
        private const string ICLOUD_URL = "https://www.icloud.com";
        private const string ICLOUD_LOGIN_URL = "https://setup.icloud.com/setup/ws/1/login";
        private const string ICLOUD_INIT_CLIENT_URL = "/fmipservice/client/web/initClient";
        private const string ICLOUD_PLAY_SOUND_URL = "/fmipservice/client/web/playSound";

        private WebClient _webClient;
        private string _iCloudBaseUrl;

        public iCloudService(string appleId, string password)
        {
            this.Authenticate(appleId, password);
        }

        private void Authenticate(string appleId, string password)
        {
            if (!string.IsNullOrEmpty(_iCloudBaseUrl))
                return;

            _webClient = new WebClient();
            _webClient.Headers.Add("Origin", ICLOUD_URL);
            _webClient.Headers.Add("Content-Type", "text/plain");

            string loginParams = $"{{\"apple_id\":\"{appleId}\",\"password\":\"{password}\",\"extended_login\":false}}";
            string loginResult;

            try
            {
                loginResult = _webClient.PostDataToWebsite(ICLOUD_LOGIN_URL, loginParams);
            }
            catch (System.Net.WebException)
            {
                throw new System.Security.SecurityException("Invalid username or password.");
            }

            if (_webClient.ResponseHeaders.AllKeys.Any(k => k == "Set-Cookie"))
            {
                _webClient.Headers.Add("Cookie", _webClient.ResponseHeaders["Set-Cookie"]);

                // get the base server url specific to this account (e.g. https://p03-fmipweb.icloud.com)
                JObject loginObject = JObject.Parse(loginResult);
                _iCloudBaseUrl = loginObject["webservices"]["findme"]["url"].ToString();
            }
            else
            {
                throw new System.Security.SecurityException("Invalid username or password.");
            }
        }

        private string InitClient()
        {
            string parameters = "{\"clientContext\":{\"appName\":\"iCloud Find (Web)\",\"appVersion\":\"2.0\"," +
                                 "\"timezone\":\"US/Eastern\",\"inactiveTime\":2255,\"apiVersion\":\"3.0\",\"webStats\":\"0:15\"}}";
            string result = _webClient.PostDataToWebsite(_iCloudBaseUrl + ICLOUD_INIT_CLIENT_URL, parameters);

            if (result.StartsWith("{\"userInfo\":"))
            {
                return result;
            }
            else
            {
                throw new Exception($"Could not get User Info. Response from initClient: {result}");
            }
        }

        public List<Device> GetDevices()
        {
            string userInfo = this.InitClient();

            JObject userObject = JObject.Parse(userInfo);
            IList<JToken> results = userObject["content"].Children().ToList();

            return results.Select(r => JsonConvert.DeserializeObject<Device>(r.ToString())).ToList();
        }

        public Device GetDevice(string deviceName)
        {
            List<Device> devices = this.GetDevices();

            return devices.FirstOrDefault(d => d.Name == deviceName);
        }

        public void PlaySound(string deviceName, string message)
        {
            Device device = this.GetDevice(deviceName);
            if (device == null)
                throw new ArgumentException($"Device \"{deviceName}\" was not found.");

            string playSoundParams = $"{{\"device\":\"{device.Id}\",\"subject\":\"{message}\"}}";

            _webClient.PostDataToWebsite(_iCloudBaseUrl + ICLOUD_PLAY_SOUND_URL, playSoundParams);
        }
    }
}