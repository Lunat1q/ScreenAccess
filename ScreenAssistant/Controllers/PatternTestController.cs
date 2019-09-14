using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GlobalHook.Event;
using TiqSoft.ScreenAssistant.Controllers.BindingControl;
using TiqSoft.ScreenAssistant.Core;
using TiqSoft.ScreenAssistant.Games;
using TiqSoft.ScreenAssistant.Properties;

namespace TiqSoft.ScreenAssistant.Controllers
{
    internal sealed class PatternTestController : INotifyPropertyChanged
    {
        private static PatternTestController _instance;
        private bool _running;
        private int _prevX;
        private int _prevY;
        private const int CanvasHeight = 400;
        private const int CanvasWidth = 400;
        private const int ZoomFactor = 3;
#pragma warning disable 649
        private readonly TextBlock _counterBlock;
#pragma warning restore 649
        private int _shotNumber = 0;

        public PatternTestController()
        {
#if DEBUG
            HotKeysController = new BindingController();
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'P', Toggle);
            HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'N', NewRecording);
            HotKeysController.Start(true);
            Canvas = new Canvas { Width = CanvasHeight, Height = CanvasWidth };
            _counterBlock = new TextBlock();
            NewRecording();
#endif
        }

        private void NewRecording()
        {
            Canvas.Children.Clear();
            Canvas.Children.Add(_counterBlock);
            _shotNumber = 0;
            _counterBlock.Text = _shotNumber.ToString();
            _prevX = CanvasWidth / 2;
            _prevY = CanvasHeight / 2;
            var xAxis = new Line
            {
                Width = 2,
                Height = CanvasHeight,
                Stroke = Brushes.DimGray,
                StrokeThickness = 2,
                X1 = 0,
                X2 = 0,
                Y1 = 0,
                Y2 = CanvasHeight
            };
            Canvas.Children.Add(xAxis);
            Canvas.SetLeft(xAxis, _prevX - 1);
            Canvas.SetTop(xAxis, 0);
            var yAxis = new Line
            {
                Width = CanvasWidth,
                Height = 2,
                Stroke = Brushes.DimGray,
                StrokeThickness = 2,
                X1 = 0,
                X2 = CanvasWidth,
                Y1 = 0,
                Y2 = 0
            };
            Canvas.Children.Add(yAxis);
            Canvas.SetLeft(yAxis, 0);
            Canvas.SetTop(yAxis, _prevY - 1);
        }

        public static PatternTestController Instance => _instance ?? (_instance = new PatternTestController());
        
        public Canvas Canvas { get; }

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

        public Dispatcher Dispatcher { get; set; }

        private void Toggle()
        {
            Running = !Running;
            if (Running)
            {
                if (WeaponFactory != null)
                {
                    WeaponFactory.WeaponCreated += WeaponFactoryOnWeaponCreated;
                }
            }
            else
            {
                if (WeaponFactory != null)
                {
                    WeaponFactory.WeaponCreated -= WeaponFactoryOnWeaponCreated;
                }
            }
        }

        private void WeaponFactoryOnWeaponCreated(object sender, WeaponCreatedEventArgs args)
        {
            args.Weapon.MouseMoved += WeaponOnMouseMoved;
        }

        private void WeaponOnMouseMoved(object sender, MouseMovedEventArgs args)
        {
            var x = _prevX - args.X * ZoomFactor;
            var y = _prevY - args.Y * ZoomFactor;
            Dispatcher.Invoke(() =>
            {
                _counterBlock.Text = (++_shotNumber).ToString();
                var point = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    //Stroke = Brushes.Fuchsia,
                    StrokeThickness = 2,
                    Fill = Brushes.Fuchsia
                };
                Canvas.Children.Add(point);
                Canvas.SetLeft(point, x);
                Canvas.SetTop(point, y);
            });
            _prevY = y;
            _prevX = x;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
