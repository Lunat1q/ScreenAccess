using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using TiqSoft.ScreenAssistant.Core.Settings;
using TiqSoft.ScreenAssistant.Games;
using TiqUtils.Wpf.UIBuilders;

namespace TiqSoft.ScreenAssistant.Builders
{
    internal class ScreenAssistantSettingsBuilder : SettingsAutoUI
    {
        private readonly ScreenAssistantSettings _settingsClass;
        internal ScreenAssistantSettingsBuilder(ScreenAssistantSettings settingsClass) : base(settingsClass)
        {
            this._settingsClass = settingsClass;
        }

        protected override UIElement CreatePropertyUiElement(TextBlock labelBlock, PropertyInfo prop)
        {
            if (prop.Name.Equals(nameof(ScreenAssistantSettings.SelectedGameName)))
            {
                return this.CreatedGameSelectionControl(prop);
            }
            return base.CreatePropertyUiElement(labelBlock, prop);
        }

        private ComboBox CreatedGameSelectionControl(PropertyInfo prop)
        {
            var cb = new ComboBox
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                DisplayMemberPath = nameof(Game.Name),
                SelectedValuePath = nameof(Game.Name),
                ItemsSource = GamesHelper.GetListOfSupportedGames()
            };
            var valueBinding = new Binding
            {
                Path = new PropertyPath(prop.Name)
            };
            cb.SetBinding(Selector.SelectedValueProperty, valueBinding);

            return cb;
        }


        protected override void AfterBuild(Grid grid)
        {
            var label = CreateLabel("Detect settings from config:");
            var autoDetect = CreateButton("Detect");
            autoDetect.Click += this.AutoDetectOnClick;
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Grid.SetColumn(autoDetect, 1);
            Grid.SetRow(autoDetect, grid.RowDefinitions.Count - 1);
            Grid.SetRow(label, grid.RowDefinitions.Count - 1);
            grid.Children.Add(autoDetect);
            grid.Children.Add(label);
        }

        private void AutoDetectOnClick(object sender, RoutedEventArgs e)
        {
            var settingsReader = GamesHelper.GetSettingsReaderByGameName(this._settingsClass.SelectedGameName);
            settingsReader.UpdateSettings(this._settingsClass);
            this.Close();
        }
    }
}
