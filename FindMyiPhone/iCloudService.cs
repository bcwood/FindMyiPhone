using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace FindMyiPhone
{
    public class iCloudService
    {
        private const string ICLOUD_URL = "https://www.icloud.com";
        private const string ICLOUD_LOGIN_URL = "https://setup.icloud.com/setup/ws/1/login";
        private const string ICLOUD_INIT_CLIENT_URL = "/fmipservice/client/web/initClient";
        private const string ICLOUD_PLAY_SOUND_URL = "/fmipservice/client/web/playSound";

        private readonly string _appleId;
        private readonly string _password;
        private WebClient _webClient;
        private string _iCloudDeviceUrl;

        public iCloudService(string appleId, string password)
        {
            _appleId = appleId;
            _password = password;
        }

        private void Authenticate()
        {
            if (!string.IsNullOrEmpty(_iCloudDeviceUrl))
                return;

            _webClient = new WebClient();
            _webClient.Headers.Add("Origin", ICLOUD_URL);
            _webClient.Headers.Add("Content-Type", "text/plain");

            string loginParams = $"{{\"apple_id\":\"{_appleId}\",\"password\":\"{_password}\",\"extended_login\":false}}";
            string loginResult = _webClient.PostDataToWebsite(ICLOUD_LOGIN_URL, loginParams);

            if (_webClient.ResponseHeaders.AllKeys.Any(k => k == "Set-Cookie"))
            {
                _webClient.Headers.Add("Cookie", _webClient.ResponseHeaders["Set-Cookie"]);

                var js = new JavaScriptSerializer();
                var loginData = js.Deserialize(loginResult, typeof(object)) as dynamic;
                _iCloudDeviceUrl = (string) loginData["webservices"]["findme"]["url"];
            }
            else
            {
                throw new System.Security.SecurityException("Invalid username and/or password.");
            }
        }

        public dynamic GetUserInfo()
        {
            this.Authenticate();

            string clientInitParams = "{\"clientContext\":{\"appName\":\"iCloud Find (Web)\",\"appVersion\":\"2.0\"," +
                                       "\"timezone\":\"US/Eastern\",\"inactiveTime\":2255,\"apiVersion\":\"3.0\",\"webStats\":\"0:15\"}}";
            string clientInitResult = _webClient.PostDataToWebsite(_iCloudDeviceUrl + ICLOUD_INIT_CLIENT_URL, clientInitParams);

            if (clientInitResult.StartsWith("{\"userInfo\":"))
            {
                var js = new JavaScriptSerializer();
                return js.Deserialize(clientInitResult, typeof(object));
            }

            return null;
        }

        public dynamic GetDevices()
        {
            dynamic userInfo = this.GetUserInfo();

            if (userInfo != null)
                return userInfo["content"];
            else
                return null;
        }

        public dynamic GetDevice(string deviceName)
        {
            dynamic devices = this.GetDevices();

            if (devices == null)
                return null;

            foreach (Dictionary<string, object> device in devices)
            {
                if (device.Values.Contains(deviceName))
                {
                    return device;
                }
            }

            return null;
        }

        public Location GetLocation(string deviceName)
        {
            dynamic device = this.GetDevice(deviceName);

            return new Location
            {
                Latitude = device["location"]["latitude"],
                Longitude = device["location"]["longitude"],
                Accuracy = device["location"]["horizontalAccuracy"],
                Timestamp = device["location"]["timeStamp"]
            };
        }

        public void PlaySound(string deviceName, string message)
        {
            dynamic device = this.GetDevice(deviceName);

            if (device == null)
                throw new ArgumentException($"Device \"{deviceName}\" was not found.");

            string deviceId = device["id"];
            string playSoundParams = $"{{\"device\":\"{deviceId}\",\"subject\":\"{message}\"}}";

            _webClient.PostDataToWebsite(_iCloudDeviceUrl + ICLOUD_PLAY_SOUND_URL, playSoundParams);
        }
    }
}