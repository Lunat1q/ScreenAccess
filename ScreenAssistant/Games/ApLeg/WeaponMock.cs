using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiqSoft.ScreenAssistant.Annotations;
using TiqSoft.ScreenAssistant.Core;

namespace TiqSoft.ScreenAssistant.Games.ApLeg
{
    public class WeaponMock : IWeapon
    {
        private string _name;

        public string Name
        {
            get => this._name;
            set
            {
                this._name = value;
                this.OnPropertyChanged();
            }
        }

        public double AdjustmentCoefficient { get; } = 1;

        public bool IsTheSameWeapon(string weaponName)
        {
            return true;
        }

        public double AdjustMouse(int shotNumber)
        {
            return 0;
        }
        
        public void SetSensitivityScale(float sensitivityScale)
        {
            // ignore
        }

        public bool IsDefault()
        {
            return true;
        }

        public bool IsActive { get; set; }

        public int NumberOfModules { get; } = 5;

        public event MouseMovedEvent MouseMoved;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnMouseMoved(MouseMovedEventArgs args)
        {
            this.MouseMoved?.Invoke(this, args);
        }
    }
}