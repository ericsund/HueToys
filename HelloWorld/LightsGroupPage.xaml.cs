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
        List<GroupHeader> headers = new List<GroupHeader>();

        public class GroupHeader
        {
            public GroupHeader()
            {
                Groups = new ObservableCollection<Group>();
            }

            public string Name { get; set; }
            public ObservableCollection<Group> Groups { get; private set; }
        }

        public class Group
        {
            public string NumLights { get; set; }
            public string[] LightIds { get; set; }
            public bool AllOn { get; set; }
        }

        public LightsGroupPage()
        {
            this.InitializeComponent();
            PopulateLightsData();
        }

        private void PopulateLightsData()
        {
            dynamic groupsInfo = Groups.GetGroups();
            
            int i = 1;
            while (groupsInfo[i.ToString()] != null)
            {
                GroupHeader group1 = new GroupHeader();
                group1.Name = groupsInfo[i.ToString()]["name"];

                // find all lights in the current group
                Group g1 = new Group();
                g1.AllOn = true;
                g1.LightIds = groupsInfo[i.ToString()]["lights"].ToObject<string[]>();
                g1.NumLights = groupsInfo[i.ToString()]["lights"].Count.ToString();
                group1.Groups.Add(g1);

                headers.Add(group1);
                
                i++;
            }

            cvsProjects.Source = headers;
        }

        private void GroupBrightnessChanged(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Slider MySlider = sender as Slider;
            double newBrightness = MySlider.Value;

            Group h = headers[0].Groups[0];
            foreach (string currLightId in h.LightIds)
            {
                HueAPI.Lights.SetLightBrightness(int.Parse(currLightId), (int)newBrightness);
            }
        }

        private void GroupSwitch(object sender, ItemClickEventArgs e)
        {
            Group g = headers[0].Groups[0];
            if (g.AllOn)
            {
                HueAPI.Lights.ForceAllOnOff(g, false);
            }
            else
            {
                HueAPI.Lights.ForceAllOnOff(g, true);
            }
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
