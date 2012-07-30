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
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace TileGame
{
    public partial class ShowOriginPage : PhoneApplicationPage
    {
        public BitmapSource OriBitmap
        {
            get;
            set;
        }

        public ShowOriginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            image1.Source = OriBitmap;
            base.OnNavigatedTo(e);
        }
    }
}