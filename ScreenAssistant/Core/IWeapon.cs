using System.ComponentModel;

namespace TiqSoft.ScreenAssistant.Core
{
    public interface IWeapon : INotifyPropertyChanged
    {
        string Name { get; }

        double AdjustmentCoefficient { get; }

        bool IsTheSameWeapon(string weaponName);

        double AdjustMouse(int shotNumber);

        void SetOffsets(int deltaX, int deltaY);

        void SetSensitivityScale(float sensitivityScale);

        bool IsDefault();

        bool IsActive { get; set; }

        int NumberOfModules { get; }
    }
}