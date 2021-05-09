using System;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HueAPI
{
    class APICall
    {
        private static string baseUrl = "http://192.168.0.16/api/z01OPw01ouRwbecqso7V1sFcuea4PC37pSs4lrXW/lights/";

        public static string Get()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string res = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            return res;
        }

        public static void Put(string url, string payload)
        {
            // create a new WebClient and overload a method
            using (var client = new System.Net.WebClient())
            {
                client.UploadString(url, "PUT", payload);
            }
        }
    }

    class Lights
    {
        private static readonly string baseUrl = "http://192.168.0.16/api/z01OPw01ouRwbecqso7V1sFcuea4PC37pSs4lrXW/lights/";

        public static void ToggleOnOff(int LightNum)
        {
            string url = $"{baseUrl}{LightNum.ToString()}/state";

            // get the state of the light
            string res = APICall.Get();
            dynamic resDeserialized = JsonConvert.DeserializeObject(res);
            string resParsed = resDeserialized[LightNum.ToString()]["state"]["on"].ToString();

            // turn the light on or off
            if (resParsed.Equals("True"))
            {
                APICall.Put(url, "{\"on\":false}");
            }
            else if (resParsed.Equals("False"))
            {
                APICall.Put(url, "{\"on\":true}");
            }
        }
    }
}
