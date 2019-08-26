using System.ComponentModel;
using System.Runtime.CompilerServices;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Games;
using TiqSoft.ScreenAssistant.Properties;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal sealed class ImageTestController : INotifyPropertyChanged
    {
        private static ImageTestController _instance;
        private bool _running;

        public ImageTestController()
        {
#if DEBUG
            HotKeysController = new BindingController();
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'Y', Toggle);
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'T', MakeTestScreenShots);
            HotKeysController.Start(true);
#endif
        }

        public static ImageTestController Instance => _instance ?? (_instance = new ImageTestController()); // rework to make it non-game specific

        private void MakeTestScreenShots()
        {
            if (Running)
            {
                WeaponFactory?.Recognizer.TestWeapons();
            }
        }

        private BindingController HotKeysController { get; }

        internal IWeaponFactory WeaponFactory { get; set; }

        public bool Running
        {
            get => _running;
            set
            {
                if (value == _running) return;
                _running = value;
                OnPropertyChanged();
            }
        }

        private void Toggle()
        {
            Running = !Running;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
