using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using TiqSoft.ScreenAssistant.Core.Settings;
using TiqSoft.ScreenAssistant.Games;
using TiqUtils.Wpf.Converters;
using TiqUtils.Wpf.UIBuilders;

namespace TiqSoft.ScreenAssistant.Builders
{
    internal class ScreenAssistantSettingsBuilder : SettingsAutoUI
    {
        internal ScreenAssistantSettingsBuilder(object settingsClass) : base(settingsClass)
        {
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
    }
}
