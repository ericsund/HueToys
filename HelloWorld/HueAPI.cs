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
        public static string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
            string res = APICall.Get(baseUrl);
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

        public static void ForceAllOnOff(HelloWorld.LightsGroupPage.Group g, bool on)
        {
            string res = APICall.Get(baseUrl);
            dynamic resDeserialized = JsonConvert.DeserializeObject(res);

            foreach (string currLightId in g.LightIds)
            {
                // get the state of the light    
                string resParsed = resDeserialized[currLightId.ToString()]["state"]["on"].ToString();
                // create url to change light status
                string url = $"{baseUrl}{currLightId.ToString()}/state";

                if (on)
                {
                    APICall.Put(url, "{\"on\":true}");
                    g.AllOn = true;
                }
                else
                {
                    APICall.Put(url, "{\"on\":false}");
                    g.AllOn = false;
                }
            }
        }

        public static dynamic FetchLightInfo(int LightNum)
        {
            string url = $"{baseUrl}{LightNum.ToString()}";
            string res = APICall.Get(url);
            dynamic resDeserialized = JsonConvert.DeserializeObject(res);
            return resDeserialized;
        }

        public static void SetLightBrightness(int LightNum, int NewBri)
        {
            string url = $"{baseUrl}{LightNum.ToString()}/state";
            APICall.Put(url, "{\"bri\":" + NewBri.ToString() + "}");
        }
    }

    class Groups
    {
        private static readonly string baseUrl = "http://192.168.0.16/api/z01OPw01ouRwbecqso7V1sFcuea4PC37pSs4lrXW/groups/";
        public static dynamic GetGroups()
        {
            string res = APICall.Get(baseUrl);
            dynamic resDeserialized = JsonConvert.DeserializeObject(res);
            return resDeserialized;
        }
    }
}
