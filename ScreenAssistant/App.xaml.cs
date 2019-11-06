using System;
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
        // ReSharper disable once UnusedMember.Local
        private static string CurrentDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? throw new InvalidOperationException();
        // ReSharper disable once UnusedMember.Local
        private const string LauncherName = "TiqLauncher.ScreenAssistant.exe";

#pragma warning disable 1998
        async void AppStartup(object sender, StartupEventArgs e)
#pragma warning restore 1998
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
                var launcherPath = Path.Combine(CurrentDirectory, LAUNCHER_NAME);
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
