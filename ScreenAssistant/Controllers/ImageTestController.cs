using System.ComponentModel;
using System.Runtime.CompilerServices;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Games;
using TiqSoft.ScreenAssistant.Properties;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal sealed class ImageTestController : INotifyPropertyChanged
    {
        private static ImageTestController _instance;
        private bool _running;

        public ImageTestController()
        {
#if DEBUG
            this.HotKeysController = new BindingController();
            this.HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'Y', this.Toggle);
            this.HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'T', this.MakeTestScreenShots);
            this.HotKeysController.Start(true);
#endif
        }

        public static ImageTestController Instance => _instance ?? (_instance = new ImageTestController()); // rework to make it non-game specific

        private void MakeTestScreenShots()
        {
            if (this.Running)
            {
                this.WeaponFactory?.Recognizer.TestWeapons();
            }
        }

        private BindingController HotKeysController { get; }

        internal IWeaponFactory WeaponFactory { get; set; }

        public bool Running
        {
            get => this._running;
            set
            {
                if (value == this._running) return;
                this._running = value;
                this.OnPropertyChanged();
            }
        }

        private void Toggle()
        {
            this.Running = !this.Running;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
