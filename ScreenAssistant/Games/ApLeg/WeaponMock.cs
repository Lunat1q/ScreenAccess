﻿using System.ComponentModel;
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
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
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

        public void SetOffsets(int deltaX, int deltaY)
        {
            // ignore
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
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}