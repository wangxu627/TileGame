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
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace TileGame
{
    public partial class SelectSourcePage : PhoneApplicationPage
    {
        private PhotoChooserTask photoChooser = new PhotoChooserTask();//创建一个相片选择器  用来选择一张图片
        private CameraCaptureTask cameraCapture = new CameraCaptureTask();
        private BitmapSource bitmapSource = null;
        private bool IsFromPhoto;

        public SelectSourcePage()
        {
            InitializeComponent();
            photoChooser.Completed += new EventHandler<PhotoResult>(photoChooser_Completed);
            cameraCapture.Completed += new EventHandler<PhotoResult>(photoChooser_Completed);
        }

        private void gallery_Click(object sender, RoutedEventArgs e)
        {
            IsFromPhoto = true;
            photoChooser.PixelWidth = 480;
            photoChooser.PixelHeight = 600;
            photoChooser.Show();
        }

        private void photo_Click(object sender, RoutedEventArgs e)
        {
            IsFromPhoto = false;
            cameraCapture.Show();
        }

        void photoChooser_Completed(object sender, PhotoResult e)
        {
            if (e.Error != null || e.ChosenPhoto == null)//没有选择图片则返回跳出该方法
            {
                return;
            }
            //创建BitmapImage来存储选择器的图片

            if (IsFromPhoto == false)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e.ChosenPhoto);
                Image imgBase = new Image();
                imgBase.Source = bitmapImage;

                WriteableBitmap writeableBitmap = new WriteableBitmap(480, 600);
                
                ScaleTransform scale = new ScaleTransform();
                scale.ScaleX = 480.0 / bitmapImage.PixelHeight;
                scale.ScaleY = 640.0 / bitmapImage.PixelWidth;

                RotateTransform rotate = new RotateTransform();
                rotate.Angle = 90;
                //rotate.CenterX = 4.0 / 2;
                //rotate.CenterY = 640.0 / 2;

                TranslateTransform translate = new TranslateTransform();
                translate.X = 480;
                //translate.Y = -60;

                TransformGroup transGroup = new TransformGroup();
                transGroup.Children.Add(scale);
                transGroup.Children.Add(rotate);
                transGroup.Children.Add(translate);

                writeableBitmap.Render(imgBase, transGroup);//在位图中呈现元素
                writeableBitmap.Invalidate();
                bitmapSource = writeableBitmap;
            }
            else
            {
                bitmapSource = new BitmapImage();
                bitmapSource.SetSource(e.ChosenPhoto);
            }
            //   this.NavigationService.Navigate(new Uri("/SecondPage.xaml", UriKind.Relative));
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (bitmapSource != null)
            {
                if (e.Content is GamePage)
                {
                    GamePage tmp = e.Content as GamePage;
                    tmp.TileBitmap = bitmapSource;
                    //bitmapImage.
                    
                    if (rbLevel1.IsChecked == true)
                    {
                        tmp.Row = 3;
                        tmp.Column = 3;
                        tmp.CurrentLevel = 1;
                    }
                    else if (rbLevel2.IsChecked == true)
                    {
                        tmp.Row = 4;
                        tmp.Column = 3;
                        tmp.CurrentLevel = 2;
                    }
                    else if (rbLevel3.IsChecked == true)
                    {
                        tmp.Row = 5;
                        tmp.Column = 4;
                        tmp.CurrentLevel = 3;
                    }
                }
                base.OnNavigatedFrom(e);
            }
        }
    }
}