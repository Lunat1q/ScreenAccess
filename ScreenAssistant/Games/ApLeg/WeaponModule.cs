using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiqSoft.ScreenAssistant.Annotations;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    public class WeaponModule : INotifyPropertyChanged
    {
        private WeaponModuleType _type;

        public WeaponModuleType Type
        {
            get => this._type;
            set
            {
                if (value == this._type) return;
                this._type = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}