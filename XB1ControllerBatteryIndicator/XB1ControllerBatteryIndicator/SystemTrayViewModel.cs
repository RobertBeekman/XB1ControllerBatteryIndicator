using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using SharpDX.XInput;

namespace XB1ControllerBatteryIndicator
{
    public class SystemTrayViewModel : Screen
    {
        private string _activeIcon;
        private Controller _controller;
        private string _tooltipText;

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
                _activeIcon = value;
                NotifyOfPropertyChange();
            }
        }

        public string TooltipText
        {
            get { return _tooltipText; }
            set
            {
                _tooltipText = value;
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
                if (batteryInfo.BatteryType == BatteryType.Disconnected ||
                    batteryInfo.BatteryType == BatteryType.Wired ||
                    batteryInfo.BatteryType == BatteryType.Unknown)
                {
                    TooltipText = $"Controller {_controller.UserIndex} - {batteryInfo.BatteryType}";
                    ActiveIcon = $"Resources/battery_{batteryInfo.BatteryType.ToString().ToLower()}.ico";
                }
                else
                {
                    TooltipText = $"Controller {_controller.UserIndex} - Battery level: {batteryInfo.BatteryLevel}";
                    ActiveIcon = $"Resources/battery_{batteryInfo.BatteryLevel.ToString().ToLower()}.ico";
                }
            }
            else
            {
                TooltipText = $"No controller detected";
                ActiveIcon = $"Resources/battery_disconnected.ico";
            }
            Thread.Sleep(1000);
            RefreshControllerState();
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}