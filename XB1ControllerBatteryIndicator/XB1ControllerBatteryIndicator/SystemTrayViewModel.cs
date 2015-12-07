using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using SharpDX.XInput;

namespace XB1ControllerBatteryIndicator
{
    public class SystemTrayViewModel : Screen
    {
        private string _activeIcon;
        private Controller _controller;

        public SystemTrayViewModel()
        {
            ActiveIcon = "Resources/battery_unkown.ico";
            GetController();
            Task.Run(() => RefreshControllerState());
        }

        public string ActiveIcon
        {
            get { return _activeIcon; }
            set
            {
                if (Equals(value, _activeIcon)) return;
                _activeIcon = value;
                NotifyOfPropertyChange();
            }
        }

        private void GetController()
        {
            var controllers = new[]
            {
                new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three),
                new Controller(UserIndex.Four)
            };
            _controller = controllers.FirstOrDefault(selectControler => selectControler.IsConnected);
        }

        private void RefreshControllerState()
        {
            GetController();
            if (_controller != null)
            {
                var batteryInfo = _controller.GetBatteryInformation(BatteryDeviceType.Gamepad);
                if (batteryInfo.BatteryType == BatteryType.Disconnected)
                    ActiveIcon = "Resources/battery_disconnected.ico";
                else if (batteryInfo.BatteryType == BatteryType.Wired)
                    ActiveIcon = "Resources/battery_wired.ico";
                else if (batteryInfo.BatteryType == BatteryType.Unknown)
                    ActiveIcon = "Resources/battery_unkown.ico";
                else
                    ActiveIcon = $"Resources/battery_{batteryInfo.BatteryLevel.ToString().ToLower()}.ico";
            }
            
            Thread.Sleep(1000);
            RefreshControllerState();
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        private ImageSource ToImageSource(Bitmap source)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}