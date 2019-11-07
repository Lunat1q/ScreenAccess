using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games
{
    public static class GamesHelper
    {
        private static IEnumerable<Game> _supportedGames;
        private static IEnumerable<Game> _supportedSettingsReaders;

        public static IEnumerable<Game> GetListOfSupportedGames()
        {
            if (_supportedGames == null)
            {
                var result = new ObservableCollection<Game>();
                var factoryInterface = typeof(IWeaponFactory);
                var types = Assembly.GetCallingAssembly().GetTypes().Where(x =>
                    factoryInterface.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

                foreach (var type in types)
                {
                    var gameName = type.GetCustomAttribute<GameNameAttribute>();
                    if (gameName != null)
                    {
                        result.Add(new Game(gameName.Name, type));
                    }
                }

                _supportedGames = result;
            }

            return _supportedGames;
        }

        public static IWeaponFactory GetFactoryByGameName(string name)
        {
            var type = GetListOfSupportedGames().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.FactoryType;
            if (type == null)
            {
                throw new ArgumentException("Game name is not recognized!");
            }

            return (IWeaponFactory)Activator.CreateInstance(type);
        }

        internal static ISettingsReader GetSettingsReaderByGameName(string name)
        {
            var type = GetKnownReaders().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.FactoryType;
            if (type == null)
            {
                Debug.WriteLine($"Settings reader is not found for game: {name}");
                return null;
            }

            return (ISettingsReader)Activator.CreateInstance(type);
        }

        private static IEnumerable<Game> GetKnownReaders()
        {
            if (_supportedSettingsReaders == null)
            {
                var result = new LinkedList<Game>();
                var targetInterface = typeof(ISettingsReader);
                var types = Assembly.GetCallingAssembly().GetTypes().Where(x =>
                    targetInterface.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                foreach (var type in types)
                {
                    var gameName = type.GetCustomAttribute<GameNameAttribute>();
                    if (gameName != null)
                    {
                        result.AddLast(new Game(gameName.Name, type));
                    }
                }

                _supportedSettingsReaders = result;
            }

            return _supportedSettingsReaders;
        }
    }

    public class Game
    {
        public Game(string name, Type factoryType)
        {
            this.FactoryType = factoryType;
            this.Name = name;
        }

        public string Name { get; }
        public Type FactoryType { get; }
    }
}