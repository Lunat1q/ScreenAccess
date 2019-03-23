using System.Windows;
using TiqSoft.ScreenAssistant.Controllers;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
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
            _controller = new MainLogicController(Settings.DeltaX, Settings.DeltaY, Settings.UseUniqueWeaponLogic);
            DataContext = _controller;
            InitializeComponent();
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
            Settings.Save();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            var w = WeaponTypeScreenRecognizer.IsFirstWeaponActive();

        }
    }
}
