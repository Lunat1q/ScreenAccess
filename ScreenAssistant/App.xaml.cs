using System.Windows;

namespace TiqSoft.ScreenAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void AppStartup(object sender, StartupEventArgs e)
        {
#pragma warning disable 219
            var viaLauncher = false;
#pragma warning restore 219
            for (var i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/launcher")
                {
                    viaLauncher = true;
                }
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
#if !DEBUG
            if (!viaLauncher)
            {
                mainWindow.ShowLauncherError();
            }
#endif
        }
    }
}
