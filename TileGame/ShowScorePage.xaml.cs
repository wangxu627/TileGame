using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace TileGame
{
    public partial class ShowScorePage : PhoneApplicationPage
    {
        public ShowScorePage()
        {
            InitializeComponent();

            updateUI();
        }

        private void updateUI()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Int32 time1 = Int32.Parse(settings["time1"].ToString());
            tbTime1.Text = TimeSpan.FromSeconds(time1).ToString("c");

            Int32 time2 = Int32.Parse(settings["time2"].ToString());
            tbTime2.Text = TimeSpan.FromSeconds(time2).ToString("c");

            Int32 time3 = Int32.Parse(settings["time3"].ToString());
            tbTime3.Text = TimeSpan.FromSeconds(time3).ToString("c");

            tbCount1.Text = settings["count1"].ToString();
            tbCount2.Text = settings["count2"].ToString();
            tbCount3.Text = settings["count3"].ToString();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["time1"] = 0;
            settings["time2"] = 0;
            settings["time3"] = 0;
            settings["count1"] = 0;
            settings["count2"] = 0;
            settings["count3"] = 0;

            updateUI();
        }
    }
}