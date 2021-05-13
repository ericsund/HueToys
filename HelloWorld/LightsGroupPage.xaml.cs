using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using HueAPI;
using Windows.UI;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Contacts;
using Newtonsoft.Json;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class LightsGroupPage : Page
    {
        List<LightHeader> headers = new List<LightHeader>();
        string ACTIVE_GROUP = "1";
        Boolean FLOODLIGHTS_ON;

        public class LightHeader
        {
            public LightHeader()
            {
                Lights = new ObservableCollection<LightObj>();
            }

            public string Name { get; set; }
            public string OnOffState { get; set; }
            public string LightId { get; set; }
            public string ColorloopActive { get; set; }
            public Boolean firstRainbowToggle { get; set; }
            public Boolean firstOnOffToggle { get; set; }
            public int Brightness { get; set; }

            public ObservableCollection<LightObj> Lights { get; private set; }
        }

        public class LightObj
        {
            public string LightId { get; set; }
            public string Name { get; set; }
            public string OnOffState { get; set; }
            public int Brightness { get; set; }
        }

        public static void updateOnOffStateUI(LightObj light)
        {
            string state = HueAPI.Lights.FetchLightInfo(light.LightId)["state"]["on"];

            if (state.Equals("True"))
            {
                light.OnOffState = "Switched on";
            }
            else
            {
                light.OnOffState = "Switched off";
            }
        }

        public static void toggleOnOffState(LightObj light)
        {
            HueAPI.Lights.ToggleOnOff(light.LightId);
            updateOnOffStateUI(light);
        }

        public LightsGroupPage()
        {
            this.InitializeComponent();
            PopulateAllGroupNames();
            PopulateFloodlightsToggle();
        }

        private void PopulateAllGroupNames()
        {
            dynamic groupsInfo = HueAPI.Groups.GetGroups();
            
            int i = 1;
            while (groupsInfo[i.ToString()] != null)
            {
                string currName = groupsInfo[i.ToString()]["name"];
                MenuFlyoutItem currItem = new MenuFlyoutItem();
                currItem.Name = i.ToString(); // store the Id in the Name atrribute
                currItem.Text = currName; // store the actual group name in the text attribute
                currItem.Click += SwitchGroupDisplay;

                GroupsList.Items.Add(currItem);
                i++;
            }

            // by default, display the first group from the query response
            PopulateSingleGroupControls(ACTIVE_GROUP);
        }

        private void PopulateSingleGroupControls(string GroupId)
        {
            dynamic groupsInfo = HueAPI.Groups.GetGroup(GroupId);

            // grab all the lights info in the specified group
            string[] lightIdList = groupsInfo["lights"].ToObject<string[]>();

            foreach (string lightId in lightIdList)
            {
                LightHeader currLightHeader = new LightHeader();
                string currName = HueAPI.Lights.FetchLightInfo(lightId)["name"];
                currLightHeader.Name = currName;
                currLightHeader.LightId = lightId;
                currLightHeader.Brightness = HueAPI.Lights.FetchLightInfo(lightId)["state"]["bri"];

                currLightHeader.firstOnOffToggle = true;
                currLightHeader.firstRainbowToggle = true;
                PopulateRainbowSingleLightToggle(currLightHeader);
                PopulateSingleLightToggle(currLightHeader);

                LightObj currLight = new LightObj();
                currLight.LightId = lightId;
                currLight.Brightness = HueAPI.Lights.FetchLightInfo(lightId)["state"]["bri"];
                updateOnOffStateUI(currLight);

                currLightHeader.Lights.Add(currLight);
                headers.Add(currLightHeader);
            }

            ACTIVE_GROUP = GroupId;
            Debug.WriteLine("populating group controls for " + GroupId);

            cvsProjects.Source = headers;
        }

        private void PopulateFloodlightsToggle()
        {
            if (HueAPI.Groups.AllOn(ACTIVE_GROUP))
            {
                FLOODLIGHTS_ON = true;
            }
        }

        private void PopulateSingleLightToggle(LightHeader light)
        {
            dynamic currLightInfo = HueAPI.Lights.FetchLightInfo(light.LightId);
            light.OnOffState = currLightInfo["state"]["on"].ToString();
        }

        private void PopulateRainbowSingleLightToggle(LightHeader light)
        {
            dynamic currLightInfo = HueAPI.Lights.FetchLightInfo(light.LightId);
            string currEffect = currLightInfo["state"]["effect"];
            light.ColorloopActive = currEffect.Equals("colorloop") ? "True" : "False";
        }

        private void SwitchGroupDisplay(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            PopulateSingleGroupControls(menuItem.Name);
        }

        /*        private void GroupBrightnessChanged(object sender, ManipulationCompletedRoutedEventArgs e)
                {
                    Slider MySlider = sender as Slider;
                    double newBrightness = MySlider.Value;

                    Group h = headers[0].Groups[0];
                    foreach (string currLightId in h.LightIds)
                    {
                        HueAPI.Lights.SetLightBrightness(int.Parse(currLightId), (int)newBrightness);
                    }
                }*/

        private void SingleSwitch(object sender, ItemClickEventArgs e)
        {
            LightObj res = e.ClickedItem as LightObj;
            // todo the UI does not update
            toggleOnOffState(res);
        }

        private void SingleBrightnessChanged(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Slider MySlider = sender as Slider;
            string LightId = MySlider.AccessKey;

            double newBrightness = MySlider.Value;

            HueAPI.Lights.SetLightBrightness(LightId, newBrightness.ToString());
        }

        private void SingleLight_Toggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch MyToggle = sender as ToggleSwitch;
            string currLightId = MyToggle.AccessKey;

            foreach (LightHeader header in headers)
            {
                if (header.LightId.Equals(currLightId))
                {
                    bool lightOn = HueAPI.Lights.FetchLightInfo(header.LightId)["state"]["on"].ToString().Equals("True");
                    if (header.firstOnOffToggle && !lightOn)
                    {
                        header.firstOnOffToggle = false;
                        HueAPI.Lights.ToggleOnOff(MyToggle.AccessKey);
                    }
                    else if (header.firstOnOffToggle)
                    {
                        header.firstOnOffToggle = false;
                    }
                    else
                    {
                        HueAPI.Lights.ToggleOnOff(MyToggle.AccessKey);
                    }
                }
            }
        }

        private void Rainbow_Toggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch MyToggle = sender as ToggleSwitch;
            string currLightId = MyToggle.AccessKey;

            foreach (LightHeader header in headers)
            {
                if (header.LightId.Equals(currLightId))
                {
                    bool noEffect = HueAPI.Lights.FetchLightInfo(header.LightId)["state"]["effect"].ToString().Equals("none");
                    if (header.firstRainbowToggle && noEffect)
                    {
                        header.firstRainbowToggle = false;
                        HueAPI.Lights.ToggleRainbowMode(MyToggle.AccessKey);
                    }
                    else if (header.firstRainbowToggle)
                    {
                        header.firstRainbowToggle = false;
                    }
                    else
                    {
                        HueAPI.Lights.ToggleRainbowMode(MyToggle.AccessKey);
                    }
                }
            }
        }

        private void Floodlights_Toggle(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem MyToggle = sender as MenuFlyoutItem;

            dynamic groupInfo = HueAPI.Groups.GetGroup(ACTIVE_GROUP);
            string[] lightIdList = groupInfo["lights"].ToObject<string[]>();

            FLOODLIGHTS_ON = FLOODLIGHTS_ON == true ? false : true;
            HueAPI.Lights.ForceAllOnOff(lightIdList, FLOODLIGHTS_ON);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            SideMenu.IsPaneOpen = !SideMenu.IsPaneOpen;
        }

        private void LightGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(LightGroupsPage));
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
