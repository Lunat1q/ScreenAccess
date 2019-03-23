using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

namespace TiqLauncher.ScreenAssistant
{
    class Program
    {
        private const string ExeExt = ".exe";
        private const string DefaultName = "ScreenAssistant.exe";
        private static readonly Random Random = new Random();
        private const string SecretSubFolder = "bin";
        private static string CurrentDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? throw new InvalidOperationException();


        static void Main(string[] args)
        {

            PrepareBinaries();
#if !DEBUG
            var lastName = Properties.Settings.Default.ScreenAssistantName;
            if (!RunSecretly(lastName)) RunSecretly(DefaultName);
#else
            RunSecretly(DefaultName);
#endif

        }

        private static DirectorySecurity GetAdminOnlyAccessRule()
        {
            FileSystemAccessRule administratorRule = new FileSystemAccessRule("Administrators", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
            
            DirectorySecurity dirSec = new DirectorySecurity();
            dirSec.AddAccessRule(administratorRule);
            return dirSec;
        }

        private static void PrepareBinaries()
        {
            var launcherAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
            Directory.CreateDirectory(Path.Combine(CurrentDirectory, SecretSubFolder), GetAdminOnlyAccessRule());
            foreach (var file in Directory.GetFiles(CurrentDirectory).Where(x => !Path.GetFileName(x).StartsWith(launcherAssemblyName)))
            {
                var fileName = Path.GetFileName(file);
                var fileDirectory = Path.GetDirectoryName(file);
                var targetName = Path.Combine(fileDirectory, SecretSubFolder, fileName);
                if (File.Exists(targetName))
                {
                    File.Delete(targetName);
                }
                File.Move(file, targetName);
            }
            foreach (var file in Directory.GetDirectories(CurrentDirectory).Where(x => !Path.GetFileName(x).StartsWith(launcherAssemblyName) && Path.GetFileName(x) != SecretSubFolder))
            {
                var fileName = Path.GetFileName(file);
                var fileDirectory = Path.GetDirectoryName(file);
                var targetName = Path.Combine(fileDirectory, SecretSubFolder, fileName);
                if (Directory.Exists(targetName))
                {
                    Directory.Delete(targetName, true);
                }
                Directory.Move(file, targetName);
            }
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        private static bool RunSecretly(string name)
        {
            var currentPath = Path.Combine(CurrentDirectory, SecretSubFolder, name);
            var targetExeFile = new FileInfo(currentPath);
            if (targetExeFile.Exists)
            {
                var newName = RandomString(12) + ExeExt;
                var newPath = Path.Combine(Path.GetDirectoryName(currentPath) ?? throw new InvalidOperationException(), newName);
                targetExeFile.MoveTo(newPath);
                Properties.Settings.Default.ScreenAssistantName = newName;
                Properties.Settings.Default.Save();

                var startInfo = new ProcessStartInfo(targetExeFile.FullName);
                startInfo.UseShellExecute = false;
                var p = new Process { StartInfo = startInfo };
                p.Start();
                while (p.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(1500);
                }

                if (p != null)
                {
                    p.WaitForInputIdle();
                    SetWindowText(p.MainWindowHandle, newName);
                }

                return true;
            }
            return false;
        }

        private static string RandomString(int length, string chars = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
