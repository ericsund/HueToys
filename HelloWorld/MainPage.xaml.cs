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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<SolidColorBrush> ColorOptions = new List<SolidColorBrush>();
        private SolidColorBrush CurrentColorBrush = null;

        public MainPage()
        {
            this.InitializeComponent();

            ColorOptions.Add(new SolidColorBrush(Colors.Black));
            ColorOptions.Add(new SolidColorBrush(Colors.Red));
            ColorOptions.Add(new SolidColorBrush(Colors.Orange));
            ColorOptions.Add(new SolidColorBrush(Colors.Yellow));
            ColorOptions.Add(new SolidColorBrush(Colors.Green));
            ColorOptions.Add(new SolidColorBrush(Colors.Blue));
            ColorOptions.Add(new SolidColorBrush(Colors.Indigo));
            ColorOptions.Add(new SolidColorBrush(Colors.Violet));
            ColorOptions.Add(new SolidColorBrush(Colors.White));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            SideMenu.IsPaneOpen = !SideMenu.IsPaneOpen;
        }

        private void BrushButtonClick(object sender, object e)
        {
            // When the button part of the split button is clicked,
            // apply the selected color.
            
        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When the flyout part of the split button is opened and the user selects
            // an option, set their choice as the current color, apply it, then close the flyout.
            CurrentColorBrush = (SolidColorBrush)e.AddedItems[0];
            SelectedColorBorder.Background = CurrentColorBrush;
            
            BrushFlyout.Hide();
        }

        private void LightGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LightsGroupPage));
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
