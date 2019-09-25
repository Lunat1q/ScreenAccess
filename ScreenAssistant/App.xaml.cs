using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace TiqSoft.ScreenAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string CurrentDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? throw new InvalidOperationException();
        private const string LauncherName = "TiqLauncher.ScreenAssistant.exe";

        async void AppStartup(object sender, StartupEventArgs e)
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
                await mainWindow.ShowLauncherError();
                var launcherPath = Path.Combine(CurrentDirectory, LauncherName);
                var startInfo = new ProcessStartInfo(launcherPath)
                {
                    UseShellExecute = false,
                    Arguments = "/restart"
                };
                var p = new Process { StartInfo = startInfo };
                p.Start();
                Environment.Exit(0);
            }
#endif
        }
    }
}
