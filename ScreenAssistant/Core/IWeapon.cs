using System.ComponentModel;

namespace TiqSoft.ScreenAssistant.Core
{
    public interface IWeapon : INotifyPropertyChanged
    {
        string Name { get; }

        double AdjustmentCoefficient { get; }

        bool IsTheSameWeapon(string weaponName);

        double AdjustMouse(int shotNumber);
        
        void SetSensitivityScale(float sensitivityScale);

        bool IsDefault();

        bool IsActive { get; set; }

        int NumberOfModules { get; }

        event MouseMovedEvent MouseMoved;

        bool PossiblyOutdated();

        void Refresh();
    }
}