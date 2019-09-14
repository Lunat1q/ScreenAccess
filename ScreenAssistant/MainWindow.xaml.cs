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
            _controller = new MainLogicController(Settings, Dispatcher);
            DataContext = _controller;
            InitializeComponent();
            if (_controller.PatternController.Canvas != null)
            {
                PatternControl.Children.Add(_controller.PatternController.Canvas);
            }

            //GameSelector.ItemsSource = GamesHelper.GetListOfSupportedGames();
            //GameSelector.SelectedValue = Settings.SelectedGameName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _controller.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _controller.Stop();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Save();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //new ScreenAssistantSettingsBuilder(Settings).ShowDialog();
            Settings.OpenAutoUISettingsDialog();
        }

        private void GameSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            var game = (Game)cb.SelectedItem;
            _controller.SetGameFactory(GamesHelper.GetFactoryByGameName(game.Name));
            Settings.SelectedGameName = game.Name;
        }

        public void ShowLauncherError()
        {
            ErrorPanel.Visibility = Visibility.Visible;
            ErrorMessage.Text = "Please restart via TiQ Launcher";

            Task.Run(async () =>
            {
                await Task.Delay(10000);
                Environment.Exit(0);
            });
        }
    }
}
