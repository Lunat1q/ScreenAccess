using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TiqSoft.ScreenAssistant.Builders;
using TiqSoft.ScreenAssistant.Controllers;
using TiqSoft.ScreenAssistant.Games;
using TiqUtils.Wpf.UIBuilders;
using static TiqSoft.ScreenAssistant.Core.Settings.ScreenAssistantSettings;

namespace TiqSoft.ScreenAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class MainWindow
    {
        private readonly MainLogicController _controller;

        public MainWindow()
        {
            this._controller = new MainLogicController(Settings, this.Dispatcher);
            this.DataContext = this._controller;
            this.InitializeComponent();
            if (this._controller.PatternController.Canvas != null)
            {
                this.PatternControl.Children.Add(this._controller.PatternController.Canvas);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._controller.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this._controller.Stop();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Save();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.OpenAutoUISettingsDialog();
        }
        
        public void ShowLauncherError()
        {
            this.ErrorPanel.Visibility = Visibility.Visible;
            this.ErrorMessage.Text = "Please restart via TiQ Launcher";

            Task.Run(async () =>
            {
                await Task.Delay(10000);
                Environment.Exit(0);
            });
        }
    }
}
