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
            this.HotKeysController = new BindingController();
            this.HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'P', this.Toggle);
            this.HotKeysController.BindUpToAction(KeyModifier.Ctrl, 'N', this.NewRecording);
            this.HotKeysController.Start(true);
            this.Canvas = new Canvas { Width = CanvasHeight, Height = CanvasWidth };
            this._counterBlock = new TextBlock();
            this.NewRecording();
#endif
        }

        private void NewRecording()
        {
            this.Canvas.Children.Clear();
            this.Canvas.Children.Add(this._counterBlock);
            this._shotNumber = 0;
            this._counterBlock.Text = this._shotNumber.ToString();
            this._prevX = CanvasWidth / 2;
            this._prevY = CanvasHeight / 2;
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
            this.Canvas.Children.Add(xAxis);
            Canvas.SetLeft(xAxis, this._prevX - 1);
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
            this.Canvas.Children.Add(yAxis);
            Canvas.SetLeft(yAxis, 0);
            Canvas.SetTop(yAxis, this._prevY - 1);
        }

        public static PatternTestController Instance => _instance ?? (_instance = new PatternTestController());
        
        public Canvas Canvas { get; }

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

        public Dispatcher Dispatcher { get; set; }

        private void Toggle()
        {
            this.Running = !this.Running;
            if (this.Running)
            {
                if (this.WeaponFactory != null)
                {
                    this.WeaponFactory.WeaponCreated += this.WeaponFactoryOnWeaponCreated;
                }
            }
            else
            {
                if (this.WeaponFactory != null)
                {
                    this.WeaponFactory.WeaponCreated -= this.WeaponFactoryOnWeaponCreated;
                }
            }
        }

        private void WeaponFactoryOnWeaponCreated(object sender, WeaponCreatedEventArgs args)
        {
            args.Weapon.MouseMoved += this.WeaponOnMouseMoved;
        }

        private void WeaponOnMouseMoved(object sender, MouseMovedEventArgs args)
        {
            var x = this._prevX - args.X * ZoomFactor;
            var y = this._prevY - args.Y * ZoomFactor;
            this.Dispatcher.Invoke(() =>
            {
                this._counterBlock.Text = (++this._shotNumber).ToString();
                var point = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    //Stroke = Brushes.Fuchsia,
                    StrokeThickness = 2,
                    Fill = Brushes.Fuchsia
                };
                this.Canvas.Children.Add(point);
                Canvas.SetLeft(point, x);
                Canvas.SetTop(point, y);
            });
            this._prevY = y;
            this._prevX = x;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
