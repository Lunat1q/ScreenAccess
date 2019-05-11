namespace TiqSoft.ScreenAssistant.Core
{
    public interface IWeapon
    {
        string Name { get; }

        double AdjustmentCoefficient { get; }

        bool IsTheSameWeapon(string weaponName);

        double AdjustMouse(int shotNumber);

        void SetOffsets(int deltaX, int deltaY);

        void SetSensitivityScale(float sensitivityScale);

        bool IsDefault();
    }
}