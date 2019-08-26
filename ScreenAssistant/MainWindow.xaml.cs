using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TiqSoft.ScreenAssistant.Controllers;
using TiqSoft.ScreenAssistant.Games;
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
            _controller = new MainLogicController(LogicSettings.ConstructFromSettings(Settings), Dispatcher);
            DataContext = _controller;
            InitializeComponent();
            GameSelector.ItemsSource = GamesHelper.GetListOfSupportedGames();
            GameSelector.SelectedValue = Settings.SelectedGameName;
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
            Settings.DeltaX = _controller.DeltaX;
            Settings.DeltaY = _controller.DeltaY;
            Settings.UseUniqueWeaponLogic = _controller.UseWeaponLogic;
            Settings.SensitivityScale = _controller.SensitivityScale;
            Settings.Save();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var scaleY = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
            };
            SettingsGrid.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var downScaleY = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
            };
            SettingsGrid.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, downScaleY);
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
