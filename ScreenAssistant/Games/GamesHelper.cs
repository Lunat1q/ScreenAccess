using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games
{
    public static class GamesHelper
    {
        private static IEnumerable<Game> _supportedGames;

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