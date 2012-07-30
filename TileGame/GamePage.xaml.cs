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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using Microsoft.Devices;

namespace TileGame
{
    public partial class GamePage : PhoneApplicationPage
    {
        public BitmapSource TileBitmap { get; set;}

        public Int32 Row { get; set; }

        public Int32 Column { get; set; }

        public Int32 CurrentLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private List<Image> mImageList = new List<Image>();
        private List<BitmapSource> mBitmapSource = new List<BitmapSource>();
        private Tile mTile;
        BitmapImage bi = new BitmapImage(new Uri("Background.png", UriKind.Relative));
        private Double FrameWidth;
        private Double FrameHeight;
        private bool mIsVertical;
        private Double mOriValue;
        private Object mOriObject;
        private DispatcherTimer mTimer = null;
        private Int32 mPassSeconds = 0;
        private Int32 mStepCnt = 0;
        
        /// <summary>
        /// 
        /// </summary>
        public GamePage()
        {
            InitializeComponent();

            BackKeyPress += new EventHandler<System.ComponentModel.CancelEventArgs>(GamePage_BackKeyPress);

            mTimer = new DispatcherTimer();
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Interval = TimeSpan.FromSeconds(1);
            mTimer.Start();
        }
        
        private void GamePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VibrateController vc = VibrateController.Default;
            vc.Start(TimeSpan.FromMilliseconds(100));
            if (MessageBoxResult.OK != MessageBox.Show("正在游戏中，要退出吗？", "提示", MessageBoxButton.OKCancel))
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            
            FrameWidth = canvasPanel.Width / Column;
            FrameHeight = canvasPanel.Height / Row;

            mTile = new Tile(Column,Row);
            mTile.Shuffle();

            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    Image img = new Image();
                    img.Stretch = Stretch.Fill;
                    img.Width = FrameWidth - 2;
                    img.Height = FrameHeight - 2;
                    img.RenderTransform = new TranslateTransform();
                    Canvas.SetTop(img, i * FrameHeight);
                    Canvas.SetLeft(img, j * FrameWidth);

                    Object RowObj = (Object)i;
                    Object ColObj = (Object)j;
                    img.ManipulationStarted += (obj, args) => Image_ManipulationStarted(obj, args, RowObj, ColObj);
                    canvasPanel.Children.Add(img);
                    mImageList.Add(img);
                }
            }

            Image imgBase = new Image();
            imgBase.Source = TileBitmap;
            imgBase.Stretch = Stretch.None;

            Int32 TileWidth = TileBitmap.PixelWidth / Column;
            Int32 TileHeight = TileBitmap.PixelHeight / Row;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                { 
                    WriteableBitmap writeableBitmap = new WriteableBitmap(TileWidth,TileHeight);
                    TranslateTransform translate = new TranslateTransform();
                    translate.X = -TileWidth * j;//在绘制到位图中之前应用到元素的变换
                    translate.Y = -TileHeight * i;
                    writeableBitmap.Render(imgBase, translate);//在位图中呈现元素
                    writeableBitmap.Invalidate();
                    mBitmapSource.Add(writeableBitmap);
                }
            }
            refreshContent();
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            ++mPassSeconds;
            txtTime.Text = TimeSpan.FromSeconds(mPassSeconds).ToString("c");
        }

        void Image_ManipulationStarted(object sender, ManipulationStartedEventArgs e, Object row, Object col)
        {
            Int32 aX = (Int32)col + 1;
            Int32 aY = (Int32)row + 1;

            Direction direction;
            if ((direction = mTile.ValidClick(aX, aY)) != Direction.None)
            {
                Debug.WriteLine(mTile.ValidClick(aX,aY));
                mTile.HandleClick(aX,aY);
                applyAnimation(sender,direction);
                txtCount.Text = (++mStepCnt).ToString();
            }
        }

        private void applyAnimation(object sender,Direction direction)
        {
            Image img = sender as Image;
            mOriObject = img;
            TranslateTransform rotateTransform = img.RenderTransform as TranslateTransform;

            DoubleAnimation anima = new DoubleAnimation();
            anima.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            //anima.FillBehavior = FillBehavior.Stop;
            anima.Completed += new EventHandler(anima_Completed);
            Storyboard storyboard = new Storyboard();
            //Storyboard.SetTarget(anima, rotateTransform);
            Storyboard.SetTarget(anima, img);
            if (direction == Direction.Left || direction == Direction.Right)
            {
                mIsVertical = false;
                mOriValue = Canvas.GetLeft(img);
                anima.From = mOriValue;
                anima.To = (direction == Direction.Left ? 1 : -1) * FrameWidth + anima.From;
            //    Storyboard.SetTargetProperty(anima, new PropertyPath(TranslateTransform.XProperty));
                Storyboard.SetTargetProperty(anima, new PropertyPath(Canvas.LeftProperty));
            }
            else if (direction == Direction.Up || direction == Direction.Down)
            {
                mIsVertical = true;
                mOriValue = Canvas.GetTop(img);
                anima.From = mOriValue;
                anima.To = (direction == Direction.Up ? 1 : -1) * FrameHeight + anima.From;
            //    Storyboard.SetTargetProperty(anima, new PropertyPath(TranslateTransform.YProperty));
                Storyboard.SetTargetProperty(anima, new PropertyPath(Canvas.TopProperty));
            }

            storyboard.Children.Add(anima);
            
            storyboard.Begin();
        }

        void anima_Completed(object sender, EventArgs e)
        {
            Image img = mOriObject as Image;
            if (mIsVertical)
            {
                Canvas.SetTop(img, mOriValue);
            }
            else
            {
                Canvas.SetLeft(img, mOriValue);
            }
            refreshContent();

            bool tmp = mTile.CheckFinish();

            if (tmp)
            {
                //MessageBox.Show("恭喜您顺利完成拼图","游戏完成",MessageBoxButton.OK);
                handleFinish();
            }
        }

        private void handleFinish()
        {
            mTimer.Stop();

            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

            String output = "恭喜您顺利完成了拼图";
            bool newRecord = false;

            String key1 = null;
            String key2 = null;
            if (CurrentLevel == 1)
            {
                key1 = "time1";
                key2 = "count1";
            }
            else if (CurrentLevel == 2)
            {
                key1 = "time2";
                key2 = "count2";
            }
            else
            {
                key1 = "time3";
                key2 = "count3";
            }

            if ((Int32)setting[key1] > mPassSeconds ||
            (Int32)setting[key1] == 0)
            {
                setting[key1] = mPassSeconds;
                newRecord = true;
            }
            if ((Int32)setting[key2] > mStepCnt ||
                (Int32)setting[key2] == 0)
            {
                setting[key2] = mStepCnt;
                newRecord = true;
            }

            if (newRecord)
            {
                output += ",并更新了最高纪录";
            }

            MessageBox.Show(output, "提示", MessageBoxButton.OK);
            NavigationService.GoBack();
        }

        private void refreshContent()
        {
            for (int i = 0; i < Row * Column; i++)
            {
                mImageList[i].Visibility = Visibility.Visible;
                int j = mTile[i];
                if (j != -1)
                {
                    mImageList[i].Source = mBitmapSource[j];
                }
                else
                {
                    mImageList[i].Source = bi;
                }
            }
            mImageList[(mTile.Blank.Y - 1) * Column + mTile.Blank.X - 1].Visibility = Visibility.Collapsed;
        }

        private void btnShowOri_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowOriginPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is ShowOriginPage)
            {
                ShowOriginPage tmp = e.Content as ShowOriginPage;
                tmp.OriBitmap = TileBitmap;
            }
            base.OnNavigatedFrom(e);
            
        }
    }
}