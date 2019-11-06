using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Core.Settings;
using TiqUtils.TypeSpecific;
using Path = System.IO.Path;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    [GameName(GameNameAttribute.KnownGames.ApLeg)]
    internal class ApLegSettingsReader : ISettingsReader
    {
        private static readonly Guid FolderIdSavedGames = new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4");

        private const string StudioName = "Respawn";
        private const string GameFolderName = "Apex";
        private const string LocalFolderName = "local";
        private const string ProfileFolderName = "profile";
        private const string ConfigFileName = "settings.cfg";
        private const string VideoConfigFileName = "videoconfig.txt";
        private const string ProfileConfigFileName = "profile.cfg";
        
        public void UpdateSettings(ScreenAssistantSettings settings)
        {
            SHGetKnownFolderPath(FolderIdSavedGames, (uint) KnownFolderFlag.DEFAULT_PATH, IntPtr.Zero, out var pPath);

            string path = Marshal.PtrToStringUni(pPath);
            Marshal.FreeCoTaskMem(pPath);

            if (path == null)
            {
                return;
            }

            var gameSettingsFolderPath = Path.Combine(path, StudioName, GameFolderName);

            var localPath = Path.Combine(gameSettingsFolderPath, LocalFolderName);
            var profilePath = Path.Combine(gameSettingsFolderPath, ProfileFolderName);
            var settingsFilePath = Path.Combine(localPath, ConfigFileName);
            var videoConfigFilePath = Path.Combine(localPath, VideoConfigFileName);
            var profileConfigFilePath = Path.Combine(profilePath, ProfileConfigFileName);
            IEnumerable<string> settingsFromFiles = null;
            if (File.Exists(settingsFilePath))
            {
                settingsFromFiles = File.ReadAllLines(settingsFilePath);
            }

            if (File.Exists(videoConfigFilePath))
            {
                var videoSettings = File.ReadAllLines(videoConfigFilePath);
                var videoSettingsFiltered = videoSettings.Skip(2).Take(videoSettings.Length - 3);
                settingsFromFiles = settingsFromFiles?.Union(videoSettingsFiltered) ?? videoSettingsFiltered;
            }

            if (File.Exists(profileConfigFilePath))
            {
                var profileSettings = File.ReadAllLines(profileConfigFilePath);
                settingsFromFiles = settingsFromFiles?.Union(profileSettings) ?? profileSettings;
            }

            var recognizedConfig = ReadConfig(settingsFromFiles);
            recognizedConfig.UpdateSettings(settings);
        }

        private static ApLegGameConfig ReadConfig(IEnumerable<string> readAllLines)
        {
            return ApLegGameConfig.ParseSettings(readAllLines);
        }
        
        internal class ApLegGameConfig
        {
            public ApLegGameConfig()
            {
                UnknownRows = new LinkedList<ConfigRow>();
            }

            public float MouseSensitivity { get; private set; } = 5;

            public float AdsScalar0 { get; private set; }

            public bool IsFullScreen { get; private set; }

            public float FovScale { get; set; }

            public LinkedList<ConfigRow> UnknownRows { get; }

            internal static ApLegGameConfig ParseSettings(IEnumerable<string> rows)
            {
                var result = new ApLegGameConfig();

                foreach (var r in rows)
                {
                    result.ParseSettingRow(r);
                }

                return result;
            }

            private void ParseSettingRow(string s)
            {
                char[] separators = {' ', '\t'};
                char quotationSeparator = '"';
                var curIdx = 0;
                ConfigRow result = null;
                var commandFound = false;
                while (curIdx < s.Length)
                {
                    if (!commandFound)
                    {
                        var nextSpaceIdx = s.IndexOfAny(separators, curIdx);
                        var command = s.Substring(curIdx, nextSpaceIdx - curIdx);
                        if (!command.Empty())
                        {
                            result = new ConfigRow(command);
                            commandFound = true;
                        }

                        curIdx = nextSpaceIdx + 1;
                    }
                    else
                    {
                        var startQuotationIdx = s.IndexOf(quotationSeparator, curIdx);
                        if (startQuotationIdx == -1)
                        {
                            break;
                        }

                        var endQuotationIdx = s.IndexOf(quotationSeparator, startQuotationIdx + 1);
                        var arg = s.Substring(startQuotationIdx + 1, endQuotationIdx - startQuotationIdx - 1);
                        if (!arg.Empty())
                        {
                            result.Args.AddLast(arg);
                        }

                        curIdx = endQuotationIdx + 1;
                    }
                }

                if (!this.UpdateWellKnowsSettings(result))
                {
                    this.UnknownRows.AddLast(result);
                }
            }

            private bool UpdateWellKnowsSettings(ConfigRow row)
            {
                try
                {

                    switch (row.Command)
                    {
                        case "mouse_sensitivity":
                            this.MouseSensitivity = float.Parse(row.Args.First.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                            return true;
                        case "mouse_zoomed_sensitivity_scalar_0":
                            this.AdsScalar0 = float.Parse(row.Args.First.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                            return true;
                        case "setting.fullscreen":
                            this.IsFullScreen = Convert.ToByte(row.Args.First.Value) == 1;
                            return true;
                        case "cl_fovScale":
                            this.FovScale = float.Parse(row.Args.First.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                            return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to recognize {row.Command} command with arg: {row.Args.First.Value}, message: {e.Message}");
                }

                return false;
            }

            public void UpdateSettings(ScreenAssistantSettings settings)
            {
                settings.SensitivityScale = 5 / Math.Max((this.MouseSensitivity * this.AdsScalar0), 0.001f);
                settings.FullScreenMode = this.IsFullScreen;
            }
        }


        internal class ConfigRow
        {
            public ConfigRow(string command)
            {
                if (command.StartsWith("\"") && command.EndsWith("\""))
                {
                    command = command.Substring(1, command.Length - 2);
                }
                this.Command = command;
                this.Args = new LinkedList<string>();
            }

            public string Command { get; }
            public LinkedList<string> Args { get; }

            public override int GetHashCode()
            {
                return this.Command.GetHashCode(); // this is done on purpose!
            }

            public override string ToString()
            {
                return this.Command;
            }
        }

        [Flags]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private enum KnownFolderFlag : uint
        {
            None = 0x0,
            CREATE = 0x8000,
            DONT_VERFIY = 0x4000,
            DONT_UNEXPAND = 0x2000,
            NO_ALIAS = 0x1000,
            INIT = 0x800,
            DEFAULT_PATH = 0x400,
            NOT_PARENT_RELATIVE = 0x200,
            SIMPLE_IDLIST = 0x100,
            ALIAS_ONLY = 0x80000000
        }
        
        [DllImport("shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfId, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

    }


}
