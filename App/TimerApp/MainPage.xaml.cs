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
using Microsoft.Phone.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Devices;
using Coding4Fun.Phone.Controls.Toolkit;

namespace TimerApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DateTime dateTimeEnd = new DateTime();
        DateTime dateTimeRunning = new DateTime();
        List<DateTime> dateTimeEndCycles = new List<DateTime>();
        List<DateTime> dateTimeRunningCycles = new List<DateTime>();
        int currentCycle = 0;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            dispatcherTimer.Stop();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Tick += new EventHandler(timer_tick);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void ApplicationBarAddButton_Click(object sender, EventArgs e)
        {
            PivotItem pItem = new PivotItem();
            pItem.Header = "cycle " + App.ViewModel.CycleCounter++;
            TimeSpanPicker picker = new TimeSpanPicker();
            picker.Value = new TimeSpan(0);
            picker.Step = new TimeSpan(0, 0, 1);
            picker.ValueStringFormat = "{0:hh:mm:ss.fff}";
            picker.Template = (ControlTemplate)Application.Current.Resources["TimeSpanCustomStyle"];
            pItem.Content = picker;
            cyclePivot.Items.Add(pItem);
            dateTimeEndCycles.Add(new DateTime());
            dateTimeRunningCycles.Add(new DateTime());
        }

        private void ApplicationBarSettingsButton_Click(object sender, EventArgs e)
        {

        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (cyclePivot.Items.Count == 0)
            {
                MessageBox.Show("Please define at least one cycle before continuing", "No cycles have been defined", MessageBoxButton.OK);
                return;
            }
            dateTimeRunning = DateTime.Now;
            dateTimeEnd = dateTimeRunning.Add((TimeSpan)timeSpanPicker.Value);
            int i = currentCycle;
            //for (int i = 0; i < App.ViewModel.CycleCounter - 1; i++)
            //{
            DateTime dtEnd = dateTimeEndCycles[i];
            DateTime dtRunning = dateTimeRunningCycles[i];
            PivotItem pItem = (PivotItem)cyclePivot.Items[i];
            TimeSpanPicker picker = (TimeSpanPicker)pItem.Content;
            dateTimeEndCycles[i] = dateTimeRunning.Add((TimeSpan)picker.Value);
            dateTimeRunningCycles[i] = DateTime.Now;
            //}

            //Time is zero, so exit
            if (timeSpanPicker.Value == TimeSpan.FromTicks(0))
                return;

            PlayPause.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Images/appbar.transport.pause.rest.png");
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
            else
            {
                dispatcherTimer.Stop();
                PlayPause.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Images/appbar.transport.play.rest.png");
            }

        }

        private void timer_tick(object sender, EventArgs e)
        {
            dateTimeRunning = DateTime.Now;
            timeSpanPicker.Value = TimeSpan.FromTicks(dateTimeEnd.Ticks - dateTimeRunning.Ticks);
            int i = currentCycle;
            //for (int i = 0; i < App.ViewModel.CycleCounter - 1; i++)
            //{
            DateTime dtEnd = dateTimeEndCycles[i];
            DateTime dtRunning = dateTimeRunningCycles[i];
            PivotItem pItem = (PivotItem)cyclePivot.Items[i];
            TimeSpanPicker picker = (TimeSpanPicker)pItem.Content;
            picker.Value = TimeSpan.FromTicks(dtEnd.Ticks - dateTimeRunning.Ticks);
            if (picker.Value <= TimeSpan.FromTicks(0))
            {
                if (currentCycle == (App.ViewModel.CycleCounter - 1))
                {
                    MessageBox.Show("Loops here");
                }
                else
                {
                    dateTimeEndCycles[currentCycle] = dateTimeRunning.Add((TimeSpan)picker.Value);
                    dateTimeRunningCycles[currentCycle] = DateTime.Now;
                    currentCycle++;
                }
            }
            //}
            //Time has ended
            if (timeSpanPicker.Value <= TimeSpan.FromTicks(0))
            {
                VibrateController vibrateController = VibrateController.Default;
                vibrateController.Start(TimeSpan.FromSeconds(1));
                dispatcherTimer.Stop();
                timeSpanPicker.Value = TimeSpan.FromTicks(0);
                PlayPause.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Images/appbar.transport.play.rest.png");
            }
        }
    }
}