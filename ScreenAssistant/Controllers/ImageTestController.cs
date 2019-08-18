using System.ComponentModel;
using System.Runtime.CompilerServices;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Properties;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal sealed class ImageTestController : INotifyPropertyChanged
    {
        private static ImageTestController _instance;
        private bool _running;
        private readonly IWeaponRecognizer _weaponRecognizer;

        public ImageTestController(IWeaponRecognizer weaponRecognizer)
        {
            _weaponRecognizer = weaponRecognizer;
            HotKeysController = new BindingController();
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'T', MakeTestScreenShots);
        }

        public static ImageTestController Instance => _instance ?? (_instance = new ImageTestController(new ApLegWeaponTypeScreenRecognizer())); // rework to make it non-game specific

        private void MakeTestScreenShots()
        {
            _weaponRecognizer.TestWeapons();
        }

        private BindingController HotKeysController { get; }

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

        public void Toggle()
        {
            if (!Running)
            {
                HotKeysController.Start(true);
            }
            else
            {
                HotKeysController.Stop();
            }

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
